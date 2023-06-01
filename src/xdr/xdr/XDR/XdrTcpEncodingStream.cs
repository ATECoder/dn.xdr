using System.Net.Sockets;

namespace cc.isr.XDR;

/// <summary>
/// The <see cref="XdrTcpEncodingStream"/> class provides the necessary functionality to
/// <see cref="XdrEncodingStreamBase"/> to send XDR records to the network using the stream-
/// oriented TCP/IP.
/// </summary>
/// <remarks>   <para>
///
/// Remote Tea authors: Harald Albrecht, Jay Walters. </para></remarks>
public class XdrTcpEncodingStream : XdrEncodingStreamBase
{

    #region " construction and cleanup "

    /// <summary>
    /// The streaming socket to be used when receiving this XDR stream's
    /// buffer contents.
    /// </summary>
    private Socket? _socket;

    /// <summary>   The output stream used to get rid of bytes going off to the network. </summary>
    private Stream? _stream;

    /// <summary>
    /// The buffer which will be filled from the datagram socket and then
    /// be used to supply the information when decoding data.
    /// </summary>
    private byte[] _buffer;

    /// <summary>The write pointer is an index into the <see cref="_buffer"/>.</summary>
    private int _bufferIndex;

    /// <summary>Index of the last four byte word in the <see cref="_buffer"/>.</summary>
    private readonly int _bufferHighmark;

    /// <summary>Index of fragment header within <see cref="_buffer"/>.</summary>
    private int _bufferFragmentHeaderIndex;

    /// <summary>Some zeros, only needed for padding -- like in real life.</summary>
    private static readonly byte[] _paddingZeros = new byte[] { 0, 0, 0, 0 };

    /// <summary>
    /// Constructs a new <see cref="XdrTcpEncodingStream"/> object and associate it with the given 
    /// <see cref="NetworkStream"/> <see cref="Socket"/> for TCP/IP-based communication.
    /// </summary>
    /// <param name="socket">       Socket to which XDR data is sent. </param>
    /// <param name="bufferSize">   Size of packet buffer for temporarily storing outgoing XDR data. </param>
    public XdrTcpEncodingStream( Socket socket, int bufferSize )
    {
        this._socket = socket;
        this._stream = new NetworkStream( socket );

        // If the given buffer size is too small, start with a more sensible
        // size. Next, if bufferSize is not a multiple of four, round it up to
        // the next multiple of four.

        if ( bufferSize < XdrEncodingStreamBase.MinBufferSizeDefault ) bufferSize = XdrEncodingStreamBase.MinBufferSizeDefault;

        if ( (bufferSize & 3) != 0 )
        {
            bufferSize = (bufferSize + 4) & ~3;
        }

        // Set up the buffer and the buffer pointers.

        this._buffer = new byte[bufferSize];
        this._bufferFragmentHeaderIndex = 0;
        this._bufferIndex = 4;
        this._bufferHighmark = bufferSize - 4;
    }

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
    ~XdrTcpEncodingStream()
    {
        if ( this.IsDisposed ) { return; }
        this.Dispose( false );
    }

    #endregion

    #region " members "

    /// <summary>
    /// Gets the remote <see cref="IPEndPoint"/> with which the socket is communicating. 
    /// </summary>
    /// <value> The remote endpoint. </value>
    public IPEndPoint RemoteEndpoint => this._socket == null ? new IPEndPoint( IPAddress.Any, 0 ) : ( IPEndPoint ) this._socket.RemoteEndPoint;

    /// <summary>   Gets the local <see cref="IPEndPoint"/> that the <see cref="Socket"/> is using for communications. </summary>
    public IPEndPoint? LocalEndpoint => this._socket == null ? new IPEndPoint( IPAddress.Any, 0 ) : ( IPEndPoint ) this._socket.LocalEndPoint;

    #endregion

    #region " actions "

    /// <summary>
    /// Flushes this encoding XDR stream and forces any buffered output bytes to be written out.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrEncodingStreamBase.EndEncoding()"/> is that calling 
    /// it is an indication that the current record is finished and any bytes previously encoded 
    /// should immediately be written to their intended destination. 
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public override void EndEncoding()
    {
        this.Flush( true, false );
    }

    /// <summary>   Ends the current record for this encoding XDR stream. </summary>
    /// <remarks>
    /// If the <paramref name="flush"/> is <see langword="true"/> any buffered output bytes 
    /// are immediately written to their intended destination. If <paramref name="flush"/>
    /// is <see langword="false"/>, then more than one record can be pipelined, for
    /// instance, to batch several ONC/RPC calls. In this case the ONC/RPC
    /// server <b>must not</b> send a reply (with the exception for the last
    /// call in a batch, which might be trigger a reply). Otherwise, you will
    /// most probably cause an interaction deadlock between client and server.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="flush">    True to flush. </param>
    public virtual void EndEncoding( bool flush )
    {
        this.Flush( true, !flush );
    }

    /// <summary>   Flushes this object. </summary>
    /// <remarks>   2023-01-30. </remarks>
    /// <param name="lastFragment"> True to last fragment. </param>
    /// <param name="batch">        True to batch. </param>
    private void Flush( bool lastFragment, bool batch )
    {

        // Encode the fragment header. We have to take batching/pipelining
        // into account, so multiple complete fragments may be waiting in
        // the same write buffer. The variable bufferFragmentHeaderIndex
        // points to the place where we should store this fragment's header.

        int fragmentLength = this._bufferIndex - this._bufferFragmentHeaderIndex - 4;
        if ( lastFragment )
        {
            fragmentLength |= unchecked(( int ) (0x80000000));
        }
        this._buffer[this._bufferFragmentHeaderIndex] = ( byte ) ((fragmentLength) >> (24 & 0x1f));
        this._buffer[this._bufferFragmentHeaderIndex + 1] = ( byte ) ((fragmentLength) >> (16 & 0x1f));
        this._buffer[this._bufferFragmentHeaderIndex + 2] = ( byte ) ((fragmentLength) >> (8 & 0x1f));
        this._buffer[this._bufferFragmentHeaderIndex + 3] = ( byte ) fragmentLength;
        if ( !lastFragment || !batch || (this._bufferIndex >= this._bufferHighmark) )
        {
            // buffer is full, so we have to flush
            // buffer not full, but last fragment and not in batch
            // not enough space for next
            // fragment header and one int

            // Finally write the buffer's contents into the vastness of
            // network space. This has to be done when we do not need to
            // pipeline calls and if there is still enough space left in
            // the buffer for the fragment header and at least a single
            // int.

            this._stream!.Write( this._buffer, 0, this._bufferIndex );
            this._stream!.Flush();

            // Reset write pointer after the fragment header int within
            // buffer, so the next bunch of data can be encoded.

            this._bufferFragmentHeaderIndex = 0;
            this._bufferIndex = 4;
        }
        else
        {

            // Batch/pipeline several consecutive XDR records. So do not
            // flush the buffer yet to the network but instead wait for more
            // data.

            this._bufferFragmentHeaderIndex = this._bufferIndex;
            this._bufferIndex += 4;
        }
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
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The int value to be encoded. </param>
    public override void EncodeInt( int value )
    {
        if ( this._bufferIndex > this._bufferHighmark )
        {
            this.Flush( false, false );
        }

        // There's enough space in the buffer, so encode this int as
        // four bytes (French octets) in big endian order (that is, the
        // most significant byte comes first.

        this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (24 & 0x1f));
        this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (16 & 0x1f));
        this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (8 & 0x1f));
        this._buffer[this._bufferIndex++] = ( byte ) value;
    }

    /// <summary>   Encodes (aka "serializes") an <see cref="uint"/> value into an XDR stream. </summary>
    /// <remarks>
    /// An XDR <see cref="uint"/> encapsulates a 32 bits <see cref="uint"/>
    /// in 4 bytes in big endian order.
    /// </remarks>
    /// <param name="value">    The value. </param>
    public override void EncodeUInt( uint value )
    {
        if ( this._bufferIndex > this._bufferHighmark )
        {
            this.Flush( false, false );
        }

        // There's enough space in the buffer, so encode this unsigned int as
        // four bytes (French octets) in big endian order (that is, the
        // most significant byte comes first.

        this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (24 & 0x1f));
        this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (16 & 0x1f));
        this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (8 & 0x1f));
        this._buffer[this._bufferIndex++] = ( byte ) value;
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
        int padding = (4 - (length & 3)) & 3;
        int toCopy;
        while ( length > 0 )
        {
            toCopy = this._bufferHighmark - this._bufferIndex + 4;
            if ( toCopy >= length )
            {

                // The buffer has more free space than we need. So copy the
                // bytes and leave the stage.

                System.Array.Copy( value, offset, this._buffer, this._bufferIndex, length );
                this._bufferIndex += length;
                // No need to adjust "offset", because this is the last round.
                break;
            }
            else
            {

                // We need to copy more data than currently available from our
                // buffer, so we copy all we can get our hands on, then fill
                // the buffer again and repeat this until we got all we want.

                System.Array.Copy( value, offset, this._buffer, this._bufferIndex, toCopy );
                this._bufferIndex += toCopy;
                offset += toCopy;
                length -= toCopy;
                this.Flush( false, false );
            }
        }
        System.Array.Copy( _paddingZeros, 0, this._buffer, this._bufferIndex, padding );
        this._bufferIndex += padding;
    }

    #endregion

}
