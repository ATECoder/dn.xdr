
using System.Net.Sockets;
using System.IO;
namespace cc.isr.XDR;

/// <summary>
/// The <see cref="XdrTcpEncodingStream"/> class provides the necessary functionality to
/// <see cref="XdrEncodingStreamBase"/> to send XDR records to the network using the stream-
/// oriented TCP/IP.
/// </summary>
/// <remarks>   Remote Tea authors: Harald Albrecht, Jay Walters. </remarks>
public class XdrTcpEncodingStream : XdrEncodingStreamBase
{

    /// <summary>
    /// The streaming socket to be used when receiving this XDR stream's
    /// buffer contents.
    /// </summary>
    private Socket? _socket;

    /// <summary>   The output stream used to get rid of bytes going off to the network. </summary>
    private Stream _stream;

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

        if ( bufferSize < XdrEncodingStreamBase.MinBufferSize ) bufferSize = XdrEncodingStreamBase.MinBufferSize;

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

    /// <summary>   Returns the Internet address of the sender of the current XDR data. </summary>
    /// <remarks>
    /// This method should only be called after <see cref="BeginEncoding(IPAddress, int)"/>,
    /// otherwise it might return stale information.
    /// </remarks>
    /// <returns>   <see cref="IPAddress"/> of the sender of the current XDR data. </returns>
    public virtual IPAddress GetSenderAddress()
    {
        return this._socket is null ? IPAddress.None : (( IPEndPoint ) this._socket.RemoteEndPoint).Address;
    }

    /// <summary>   Returns the port number of the sender of the current XDR data. </summary>
    /// <remarks>
    /// This method should only be called after <see cref="BeginEncoding(IPAddress, int)"/>,
    /// otherwise it might return stale information.
    /// </remarks>
    /// <returns>   Port number of the sender of the current XDR data. </returns>
    public virtual int GetSenderPort()
    {
        return this._socket is null ? 0 : (( IPEndPoint ) this._socket.RemoteEndPoint ).Port;
    }

    /// <summary>   Begins encoding a new XDR record. </summary>
    /// <remarks>
    /// This typically involves resetting this encoding XDR stream back into a known state.
    /// </remarks>
    /// <param name="receiverAddress">  Indicates the receiver of the XDR data. This can be
    ///                                 <see langword="null"/> for XDR streams connected permanently to a
    ///                                 receiver (like in case of TCP/IP based XDR streams). </param>
    /// <param name="receiverPort">     Port number of the receiver. </param>
    ///
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public override void BeginEncoding( IPAddress receiverAddress, int receiverPort )
    {
    }

#if false
    /// <summary>
    /// Begin encoding with the four byte word after the fragment header,
    /// which also four bytes wide. We have to remember where we can find the fragment header as we
    /// support batching/pipelining calls, so several requests (each in its own fragment) can be
    /// simultaneously in the write buffer.
    /// 
    /// bufferFragmentHeaderIndex = bufferIndex;
    /// bufferIndex += 4;
    /// </summary>
#endif

    /// <summary>
    /// Flushes this encoding XDR stream and forces any buffered output bytes to be written out.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrEncodingStreamBase.EndEncoding()"/> is that calling 
    /// it is an indication that the current record is finished and any bytes previously encoded 
    /// should immediately be written to their intended destination. 
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public override void EndEncoding()
    {
        this.Flush( true, false );
    }

    /// <summary>   Ends the current record for this encoding XDR stream. </summary>
    /// <remarks>
    /// If the <paramref name="flush"/> is <see cref="T:true"/> any buffered output bytes 
    /// are immediately written to their intended destination. If <paramref name="flush"/>
    /// is <see cref="T:false"/>, then more than one record can be pipelined, for
    /// instance, to batch several ONC/RPC calls. In this case the ONC/RPC
    /// server <b>must not</b> send a reply (with the exception for the last
    /// call in a batch, which might be trigger a reply). Otherwise, you will
    /// most probably cause an interaction deadlock between client and server.
    /// </remarks>
    /// <param name="flush">    True to flush. </param>
    ///
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public virtual void EndEncoding( bool flush )
    {
        this.Flush( true, !flush );
    }

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

            this._stream.Write( this._buffer, 0, this._bufferIndex );
            this._stream.Flush();

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

    /// <summary>
    /// Closes this encoding XDR stream and releases any system resources associated with this stream.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrEncodingStreamBase.Close()"/> is that it closes the encoding XDR stream. 
    /// A closed XDR stream cannot perform encoding operations and cannot be reopened.
    /// </remarks>
    ///
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public override void Close()
    {
        this._buffer = Array.Empty<byte>();
        this._stream = Stream.Null;

        if ( this._socket is not null )
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
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public override void EncodeInt( int value )
    {
        if ( this._bufferIndex > this._bufferHighmark )
        {
            this.Flush( false, false );
        }

        // There's enough space in the buffer, so encode this int as
        // four bytes (french octets) in big endian order (that is, the
        // most significant byte comes first.

        this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (24 & 0x1f));
        this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (16 & 0x1f));
        this._buffer[this._bufferIndex++] = ( byte ) ((value) >> (8 & 0x1f));
        this._buffer[this._bufferIndex++] = ( byte ) value;
    }

    /// <summary>
    /// Encodes (aka "serializes") a XDR opaque value, which is represented by a vector of byte
    /// values, and starts at <paramref name="offset"/> with a length of <paramref name="length"/>.
    /// </summary>
    /// <remarks>
    /// Only the opaque value is encoded, but no length indication is preceding the opaque value, so the
    /// receiver has to know how long the opaque value will be. The encoded data is always padded to
    /// be a multiple of four. If the given length is not a multiple of four, zero bytes will be used
    /// for padding.
    /// </remarks>
    /// <param name="value">    The opaque value to be encoded in the form of a series of bytes. </param>
    /// <param name="offset">   Start offset in the data. </param>
    /// <param name="length">   the number of bytes to encode. </param>
    ///
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
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
    ~XdrTcpEncodingStream()
    {
        if ( this.IsDisposed ) { return; }
        this.Dispose( false );
    }

    #endregion

}
