using System.Net.Sockets;

namespace cc.isr.XDR;

/// <summary>
/// The <see cref="XdrUdpDecodingStream"/> class provides the necessary functionality to
/// <see cref="XdrDecodingStreamBase"/> to send XDR packets over the network using the
/// datagram-oriented UDP/IP.
/// </summary>
/// <remarks>   Remote Tea authors: Harald Albrecht, Jay Walters. </remarks>
public class XdrUdpEncodingStream : XdrEncodingStreamBase
{

    /// <summary>
    /// The datagram socket to be used when sending this XDR stream's
    /// buffer contents.
    /// </summary>
    private Socket _socket;

    /// <summary>Receiver address of current buffer contents when flushed.</summary>
    private IPAddress _receiverAddress = null;

    /// <summary>The receiver's port.</summary>
    private int _receiverPort = 0;

    /// <summary>
    /// The buffer which will receive the encoded information, before it
    /// is sent via a datagram socket.
    /// </summary>
    private byte[] _buffer;

    /// <summary>The write pointer is an index into the <see cref="_buffer"/>.</summary>
    private int _bufferIndex;

    /// <summary>Index of the last four byte word in the buffer.</summary>
    private readonly int _bufferHighmark;

    /// <summary>Some zeros, only needed for padding -- like in real life.</summary>
    private static readonly byte[] _paddingZeros = new byte[] { 0, 0, 0, 0 };

    /// <summary>
    /// Creates a new UDP-based encoding XDR stream, associated with the given datagram socket.
    /// </summary>
    /// <param name="datagramSocket">   Datagram-based socket to use to get rid of encoded data. </param>
    /// <param name="bufferSize">       Size of buffer to store encoded data before it is sent as one
    ///                                 datagram. </param>
    public XdrUdpEncodingStream( Socket datagramSocket, int bufferSize )
    {
        this._socket = datagramSocket;

        // If the given buffer size is too small, start with a more sensible
        // size. Next, if bufferSize is not a multiple of four, round it up to
        // the next multiple of four.

        if ( bufferSize < XdrEncodingStreamBase.MinBufferSize ) bufferSize = XdrEncodingStreamBase.MinBufferSize;

        if ( (bufferSize & 3) != 0 )
        {
            bufferSize = (bufferSize + 4) & ~3;
        }
        this._buffer = new byte[bufferSize];
        this._bufferIndex = 0;
        this._bufferHighmark = bufferSize - 4;
    }

    /// <summary>   Begins encoding a new XDR record. </summary>
    /// <remarks>
    /// This involves resetting this encoding XDR stream back into a known state.
    /// </remarks>
    /// <param name="receiverAddress">  Indicates the receiver of the XDR data. This can be
    ///                                 <see langword="null"/> for XDR streams connected permanently to a
    ///                                 receiver (like in case of TCP/IP based XDR streams). </param>
    /// <param name="receiverPort">     Port number of the receiver. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public override void BeginEncoding( IPAddress receiverAddress, int receiverPort )
    {
        this._receiverAddress = receiverAddress;
        this._receiverPort = receiverPort;
        this._bufferIndex = 0;
    }

    /// <summary>
    /// Flushes this encoding XDR stream and forces any buffered output bytes to be written out.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrEncodingStreamBase.EndEncoding()"/> is that calling it 
    /// is an indication that the current record is finished and any bytes previously encoded should 
    /// immediately be written to their intended destination.
    /// </remarks>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public override void EndEncoding()
    {
        IPEndPoint endPoint = new( this._receiverAddress, this._receiverPort );
        _ = this._socket.SendTo( this._buffer, this._bufferIndex, SocketFlags.None, endPoint );
    }

    /// <summary>
    /// Closes this encoding XDR stream and releases any system resources associated with this stream.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrEncodingStreamBase.Close()"/> is that it closes the encoding 
    /// XDR stream. A closed XDR stream cannot perform encoding operations and cannot be reopened.
    /// </remarks>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public override void Close()
    {
        this._buffer = null;

        if ( this._socket != null )
        {
            // Since there is a non-zero chance of getting race conditions,
            // we now first set the socket instance member to null, before
            // we close the corresponding socket. This avoids null-pointer
            // exceptions in the method which waits for connections: it is
            // possible that this method is awakened because the socket has
            // been closed before we could set the socket instance member to
            // null. Many thanks to Michael Smith for tracking down this one.
            // @atecoder: I am assuming that this also releases the stream 
            // resources.

            // @atecoder: added shutdown
            try
            {
                if ( this._socket.Connected )
                    this._socket.Shutdown( SocketShutdown.Both );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Failed socket shutdown: \n{ex} " );
            }
            Socket deadSocket = this._socket;
            this._socket = null;
            try
            {
                deadSocket.Close();
                // close is a wrapper class around dispose so this 
                // is superfluous unless the close changes.
                deadSocket.Dispose();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Failed closing the socket: \n{ex} " );
            }
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a "XDR int" value and writes it down an XDR stream.
    /// </summary>
    /// <remarks>
    /// An XDR int encapsulate a 32 bits <see langword="int"/>.
    /// This method is one of the basic methods all other methods can rely on.
    /// </remarks>
    /// <param name="value">    The int value to be encoded. </param>
    /// 
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public override void EncodeInt( int value )
    {
        if ( this._bufferIndex <= this._bufferHighmark )
        {

            // There's enough space in the buffer, so encode this int as
            // four bytes (French octets) in big endian order (that is, the
            // most significant byte comes first.

            this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (24 & 0x1f));
            this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (16 & 0x1f));
            this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (8 & 0x1f));
            this._buffer[this._bufferIndex++] = ( byte ) value;
        }
        else
        {
            throw (new XdrException( XdrException.XdrBufferOverflow ));
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a XDR opaque value, which is represented by a vector of byte
    /// values, and starts at <paramref name="offset"/> with a length of <paramref name="length"/>.
    /// </summary>
    /// <remarks>
    /// Only the opaque value is encoded, but no length indication is preceding the opaque value, so
    /// the receiver has to know how long the opaque value will be. The encoded data is always padded
    /// to be a multiple of four. If the given length is not a multiple of four, zero bytes will be
    /// used for padding.
    /// </remarks>
    /// <param name="value">    The opaque value to be encoded in the form of a series of bytes. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   the number of bytes to encode. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public override void EncodeOpaque( byte[] value, int offset, int length )
    {

        // First calculate the number of bytes needed for padding.

        int padding = (4 - (length & 3)) & 3;
        if ( this._bufferIndex <= this._bufferHighmark - (length + padding) )
        {
            System.Array.Copy( value, offset, this._buffer, this._bufferIndex, length );
            this._bufferIndex += length;
            if ( padding != 0 )
            {

                // If the length of the opaque data was not a multiple, then
                // pad with zeros, so the next write pointer points to 
                // the first byte of the 4-byte core value of the XDR coded.

                System.Array.Copy( _paddingZeros, 0, this._buffer, this._bufferIndex, padding );
                this._bufferIndex += padding;
            }
        }
        else
        {
            throw (new XdrException( XdrException.XdrBufferOverflow ));
        }
    }

    #region  " IDisposable Implementation "

    /// <summary>
    /// Releases the unmanaged resources used by the XdrDecodingStreamBase and optionally releases
    /// the managed resources.
    /// </summary>
    /// <param name="disposing">    True to release both managed and unmanaged resources; false to
    ///                             release only unmanaged resources. </param>
    protected override void Dispose( bool disposing )
    {
        try
        {
            if ( disposing )
            {
                // dispose managed state (managed objects)
            }

            // free unmanaged resources and override finalizer
            this.Close();

            // set large fields to null
        }
        finally
        {
            base.Dispose( disposing );
        }
    }

    /// <summary>   Finalizer. </summary>
    ~XdrUdpEncodingStream()
    {
        if ( this.IsDisposed ) { return; }
        this.Dispose( false );
    }

    #endregion

}

