
using System.Net.Sockets;
namespace cc.isr.XDR;

/// <summary>
/// The <see cref="XdrUdpDecodingStream"/> class provides the necessary functionality to
/// <see cref="XdrDecodingStreamBase"/> to receive XDR packets from the network using the
/// datagram-oriented UDP/IP.
/// </summary>
/// <remarks>   Remote Tea authors: Harald Albrecht, Jay Walters. </remarks>
public class XdrUdpDecodingStream : XdrDecodingStreamBase
{

    #region " construction and cleanup "

    /// <summary>
    /// The datagram socket to be used when receiving this XDR stream's
    /// buffer contents.
    /// </summary>
    private Socket? _socket;

    /// <summary>
    /// The buffer which will be filled from the datagram socket and then
    /// be used to supply the information when decoding data.
    /// </summary>
    private byte[] _buffer;

    /// <summary>The read pointer is an index into the <see cref="_buffer"/>.</summary>
    private int _bufferIndex;

    /// <summary>
    /// Index of the last four byte word in the buffer, which has been read
    /// in from the datagram socket.
    /// </summary>
    private int _bufferHighmark;

    /// <summary>
    /// Constructs a new <see cref="XdrUdpDecodingStream"/> object and associate it with the given 
    /// <paramref name="datagramSocket"/> for UDP/IP-based communication.
    /// </summary>
    /// <remarks>
    /// This constructor is typically used when communicating with servers over UDP/IP using a
    /// "connected" datagram socket.
    /// </remarks>
    /// <param name="datagramSocket">   Datagram socket from which XDR data is received. </param>
    /// <param name="bufferSize">       Size of packet buffer for storing received XDR datagrams. </param>
    public XdrUdpDecodingStream( Socket datagramSocket, int bufferSize )
    {
        this._socket = datagramSocket;

        // If the given buffer size is too small, start with a more sensible
        // size. Next, if bufferSize is not a multiple of four, round it up to
        // the next multiple of four.

        if ( bufferSize < XdrDecodingStreamBase.MinBufferSizeDefault ) bufferSize = XdrDecodingStreamBase.MinBufferSizeDefault;

        if ( (bufferSize & 3) != 0 )
        {
            bufferSize = (bufferSize + 4) & ~3;
        }
        this._buffer = new byte[bufferSize];
        this._bufferIndex = 0;
        this._bufferHighmark = -4;
    }

    /// <summary>
    /// Closes this decoding XDR stream and releases any system resources associated with this stream.
    /// </summary>
    /// <remarks>
    /// A closed XDR stream cannot perform decoding operations and cannot be reopened. <para>
    /// 
    /// This implementation frees the allocated buffer but does not close
    /// the associated datagram socket. It only throws away the reference to this socket. </para>
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public override void Close()
    {
        if ( this._socket is not null )
        {
            // Since there is a non-zero chance of getting race conditions,
            // we now first set the socket instance member to null, before
            // we close the corresponding socket. This avoids null-pointer
            // exceptions in the method which waits for connections: it is
            // possible that this method is awakened because the socket has
            // been closed before we could set the socket instance member to
            // null. Many thanks to Michael Smith for tracking down this one.

            // @atecoder: added shutdown
            Socket socket = this._socket;
            if ( socket.Connected )
                socket.Shutdown( SocketShutdown.Both );
            this._socket = null;
            socket.Close();
        }
        this._buffer = Array.Empty<byte>();
        base.Close();
    }

    #endregion

    #region " members "

    private IPEndPoint _remoteEndPoint = new IPEndPoint( IPAddress.None, 0 );
    /// <summary>
    /// Gets the remote <see cref="IPEndPoint"/> with which the socket is communicating. 
    /// </summary>
    /// <remarks>
    /// This value is valid only after <see cref="BeginDecoding()"/>, otherwise it might return stale information.
    /// </remarks>
    /// <value> The remote endpoint. </value>
    public override IPEndPoint RemoteEndPoint => this._socket == null
                                                    ? new IPEndPoint( IPAddress.None, 0)
                                                    : this._socket.RemoteEndPoint is null
                                                        ? this._remoteEndPoint
                                                        : ( IPEndPoint ) this._socket.RemoteEndPoint;

    /// <summary>   Gets the local <see cref="IPEndPoint"/> that the <see cref="Socket"/> is using for communications.. </summary>
    public IPEndPoint LocalEndpoint => this._socket == null ? new IPEndPoint( IPAddress.None, 0 ) : ( IPEndPoint ) this._socket.LocalEndPoint;

    #endregion

    #region " actions "

    /// <summary>   Initiates decoding of the next XDR record. </summary>
    /// <remarks>
    /// For UDP-based XDR decoding streams this reads in the next datagram from the network socket.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public override void BeginDecoding()
    {
        // Creates an IpEndPoint to capture the identity of the sending host.
        IPEndPoint sender = new( IPAddress.Any, 0 ); 
        EndPoint remoteEP = sender;
        _ = (this._socket?.ReceiveFrom( this._buffer, ref remoteEP ));
        this._remoteEndPoint = ( IPEndPoint ) remoteEP;
        this._bufferIndex = 0;
        this._bufferHighmark = this._buffer.Length - 4;
    }

    /// <summary>   End decoding of the current XDR record. </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrDecodingStreamBase.EndDecoding"/> is that calling it is
    /// an indication that the current record is no more interesting to the caller and any allocated
    /// data for this record can be freed. <para>
    /// 
    /// This method overrides
    /// <see cref="XdrDecodingStreamBase.EndDecoding()"/>.
    /// It does nothing more than resetting the buffer pointer back to the beginning of an empty
    /// buffer, so attempts to decode data will fail until the buffer is filled again. </para>
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public override void EndDecoding()
    {
        this._bufferIndex = 0;
        this._bufferHighmark = -4;
    }

    #endregion

    #region " decoding "

    /// <summary>   Decodes (aka "deserializes") a "XDR int" value received from an XDR stream. </summary>
    /// <remarks>
    /// An XDR int encapsulate a 32 bits <see langword="int"/>.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <returns>   The decoded int value. </returns>
    public override int DecodeInt()
    {
        if ( this._bufferIndex <= this._bufferHighmark )
        {

            // There's enough space in the buffer to hold at least one
            // XDR int. So let's retrieve it now.
            // Note: buffer[...] gives a byte, which is signed. So if we
            // add it to the value (which is int), it has to be widened
            // to 32 bit, so its sign is propagated. To avoid this sign
            // madness, we have to "and" it with 0xFF, so all unwanted
            // bits are cut off after sign extension. Sigh.

            int value = this._buffer[this._bufferIndex++];
            value = (value << 8) + (this._buffer[this._bufferIndex++] & unchecked(( int ) (0xFF)));
            value = (value << 8) + (this._buffer[this._bufferIndex++] & unchecked(( int ) (0xFF)));
            value = (value << 8) + (this._buffer[this._bufferIndex++] & unchecked(( int ) (0xFF)));
            return value;
        }
        else
        {
            throw (new XdrException( XdrExceptionReason.XdrBufferUnderflow ));
        }
    }

    /// <summary>
    /// Decodes (aka "deserializes") an opaque value, which is nothing more than a series of octets
    /// (or 8 bits wide bytes).
    /// </summary>
    /// <remarks>
    /// Because the length of the opaque value is given, we don't need to retrieve it from the XDR
    /// stream. This is different from <see cref="DecodeOpaque(byte[], int, int)"/>
    /// where first the length of the opaque value is retrieved from the XDR stream.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="length">   Length of opaque data to decode. </param>
    /// <returns>   Opaque data as a byte vector. </returns>
    public override byte[] DecodeOpaque( int length )
    {

        // First make sure that the length is always a multiple of four.

        int alignedLength = length;
        if ( (alignedLength & 3) != 0 )
        {
            alignedLength = (alignedLength & ~3) + 4;
        }

        // Now allocate enough memory to hold the data to be retrieved.

        byte[] bytes = new byte[length];
        if ( length > 0 )
        {
            if ( this._bufferIndex <= this._bufferHighmark - alignedLength + 4 )
            {
                System.Array.Copy( this._buffer, this._bufferIndex, bytes, 0, length );
            }
            else
            {
                throw (new XdrException( XdrExceptionReason.XdrBufferUnderflow ));
            }
        }
        this._bufferIndex += alignedLength;
        return bytes;
    }

    /// <summary>
    /// Decodes (aka "deserializes") a XDR opaque value, which is represented by a vector of <see langword="byte"/>
    /// values, and starts at <paramref name="offset"/> with a length of <paramref name="length"/>.
    /// </summary>
    /// <remarks>
    /// Only the opaque value is decoded, so the caller has to know how long the opaque value will be. The
    /// decoded data is always padded to be a multiple of four (because that's what the sender does).
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="opaque">   Byte vector which will receive the decoded opaque value. </param>
    /// <param name="offset">   Start offset in the byte vector. </param>
    /// <param name="length">   the number of bytes to decode. </param>
    public override void DecodeOpaque( byte[] opaque, int offset, int length )
    {

        // First make sure that the length is always a multiple of four.

        int alignedLength = length;
        if ( (alignedLength & 3) != 0 )
        {
            alignedLength = (alignedLength & ~3) + 4;
        }

        // Now allocate enough memory to hold the data to be retrieved.

        if ( length > 0 )
        {
            if ( this._bufferIndex <= this._bufferHighmark - alignedLength + 4 )
            {
                System.Array.Copy( this._buffer, this._bufferIndex, opaque, offset, length );
            }
            else
            {
                throw (new XdrException( XdrExceptionReason.XdrBufferUnderflow ));
            }
        }
        this._bufferIndex += alignedLength;
    }

    #endregion

}
