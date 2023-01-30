using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace cc.isr.XDR;

/// <summary>
/// The <see cref="XdrUdpDecodingStream"/> class provides the necessary functionality to
/// <see cref="XdrDecodingStreamBase"/> to send XDR packets over the network using the
/// datagram-oriented UDP/IP.
/// </summary>
/// <remarks>   <para>
///
/// Remote Tea authors: Harald Albrecht, Jay Walters. </para></remarks>
public class XdrUdpEncodingStream : XdrEncodingStreamBase
{

    #region " construction and cleanup "

    /// <summary>
    /// The datagram socket to be used when sending this XDR stream's
    /// buffer contents.
    /// </summary>
    private Socket? _socket;

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

        if ( bufferSize < XdrEncodingStreamBase.MinBufferSizeDefault ) bufferSize = XdrEncodingStreamBase.MinBufferSizeDefault;

        if ( (bufferSize & 3) != 0 )
        {
            bufferSize = (bufferSize + 4) & ~3;
        }
        this._buffer = new byte[bufferSize];
        this._bufferIndex = 0;
        this._bufferHighmark = bufferSize - 4;
    }

    /// <summary>   The input stream used to pull the bytes off the network. </summary>
    private Stream? _stream;

    /// <summary>
    /// Releases unmanaged, large objects and (optionally) managed resources used by this class.
    /// </summary>
    /// <exception cref="AggregateException">   Thrown when an Aggregate error condition occurs. </exception>
    /// <param name="disposing">    True to release large objects and managed and unmanaged resources;
    ///                             false to release only unmanaged resources and large objects. </param>
    protected override void Dispose( bool disposing )
    {
        List<Exception> exceptions = new();
        if ( disposing )
        {
            // dispose managed state (managed objects)

            IDisposable? networkStream = this._stream;
            if ( networkStream is not null )
            {
                try
                {
                    networkStream.Dispose();
                }
                catch ( Exception ex )
                { exceptions.Add( ex ); }
                finally
                {
                    this._stream = null;
                }
            }
            else
            {
                //
                // if the NetworkStream wasn't created, the Socket might
                // still be there and needs to be closed. In the case in which
                // we are bound to a local IPEndPoint this will remove the
                // binding and free up the IPEndPoint for later uses.

                Socket? socket = this._socket;
                if ( socket is not null )
                {
                    try
                    {
                        if ( socket.Connected )
                            socket.Shutdown( SocketShutdown.Both );
                    }
                    catch ( Exception ex )
                    { exceptions.Add( ex ); }
                    finally
                    {
                        socket.Close();
                        this._socket = null;
                    }
                }
            }
        }

        // free unmanaged resources and override finalizer

        // set large fields to null

        this._buffer = Array.Empty<byte>();

        try
        {
            base.Dispose( disposing );
        }
        catch ( Exception ex )
        { exceptions.Add( ex ); }
        finally
        {
        }

        if ( exceptions.Any() )
        {
            AggregateException aggregateException = new( exceptions );
            throw aggregateException;
        }
    }


    /// <summary>   Finalizer. </summary>
    ~XdrUdpEncodingStream()
    {
        if ( this.IsDisposed ) { return; }
        this.Dispose( false );
    }

    #endregion

    #region " members "

    /// <summary>
    /// Gets the remote <see cref="IPEndPoint"/> with which the socket is communicating. 
    /// </summary>
    /// <remarks>
    /// This value is valid only after <see cref="BeginEncoding"/>, otherwise it might return stale information.
    /// </remarks>
    /// <value> The remote endpoint. </value>
    public IPEndPoint RemoteEndpoint { get; private set; } = new IPEndPoint( IPAddress.None, 0 );

    /// <summary>   Gets the local <see cref="IPEndPoint"/> that the <see cref="Socket"/> is using for communications.. </summary>
    public IPEndPoint LocalEndpoint => this._socket == null ? new IPEndPoint( IPAddress.None, 0 ) : ( IPEndPoint ) this._socket.LocalEndPoint;

    #endregion

    #region " actions "

    /// <summary>   Begins an encoding. </summary>
    /// <remarks>   This resets this encoding XDR stream back into a known initial state. </remarks>
    /// <param name="remoteEndPoint">   Indicates the remote end point of the receiver of the XDR
    ///                                 data. </param>
    public override void BeginEncoding( IPEndPoint remoteEndPoint )
    {
        this.RemoteEndpoint = new IPEndPoint( remoteEndPoint.Address, remoteEndPoint.Port );
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
    public override void EndEncoding()
    {
        _ = this._socket?.SendTo( this._buffer, this._bufferIndex, SocketFlags.None, this.RemoteEndpoint );
    }

    #endregion

    #region " encode actions "

    /// <summary>
    /// Encodes (aka "serializes") an <see cref="int"/> value into an XDR stream.
    /// </summary>
    /// <remarks>
    /// An XDR int encapsulate a 32 bits <see cref="int"/>.
    /// This method is one of the basic methods all other methods can rely on.
    /// </remarks>
    /// <param name="value">    The int value to be encoded. </param>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
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
            throw (new XdrException( XdrExceptionReason.XdrBufferOverflow ));
        }
    }

    /// <summary>   Encodes (aka "serializes") an <see cref="uint"/> value into an XDR stream. </summary>
    /// <remarks>
    /// An XDR <see cref="uint"/> encapsulates a 32 bits <see cref="uint"/>
    /// in 4 bytes in big endian order.
    /// </remarks>
    /// <exception cref="XdrException"> Thrown when an Xdr error condition occurs. </exception>
    /// <param name="value">    The value. </param>
    public override void EncodeUInt( uint value )
    {
        if ( this._bufferIndex <= this._bufferHighmark )
        {

            // There's enough space in the buffer, so encode this unsigned int as
            // four bytes (French octets) in big endian order (that is, the
            // most significant byte comes first.

            this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (24 & 0x1f));
            this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (16 & 0x1f));
            this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (8 & 0x1f));
            this._buffer[this._bufferIndex++] = ( byte ) value;
        }
        else
        {
            throw (new XdrException( XdrExceptionReason.XdrBufferOverflow ));
        }
    }

    /// <summary>
    /// Encodes (aka "serializes") a fixed-length XDR opaque data, which are represented by an 
    /// array of <see cref="byte"/> values, and starts at <paramref name="offset"/> with a 
    /// length of <paramref name="length"/> into an XDR stream.
    /// </summary>
    /// <remarks>
    /// <paramref name="length"/> array elements starting at <paramref name="offset"/> are
    /// copied into the XDR stream. <para>
    /// 
    /// Because the opaque data are encoded without its length information, the receiver has to know 
    /// how long the opaque data is. The encoded data is always padded to be a multiple of four. 
    /// If the given length is not a multiple of four, zero bytes are used for padding. </para>
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque data to be encoded in the form of a series of bytes. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   the number of bytes to encode. </param>
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
            throw (new XdrException( XdrExceptionReason.XdrBufferOverflow ));
        }
    }

    #endregion
}

