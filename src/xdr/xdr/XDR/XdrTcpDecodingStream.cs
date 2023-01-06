
using System.Net.Sockets;
using System.IO;
namespace cc.isr.XDR;

/// <summary>
/// The <see cref="XdrTcpDecodingStream"/> class provides the necessary functionality to
/// <see cref="XdrDecodingStreamBase"/> to receive XDR records from the network using the stream-
/// oriented TCP/IP.
/// </summary>
/// <remarks>   Remote Tea authors: Harald Albrecht, Jay Walters. </remarks>
public class XdrTcpDecodingStream : XdrDecodingStreamBase
{

    /// <summary>
    /// The streaming socket to be used when receiving this XDR stream's
    /// buffer contents.
    /// </summary>
    private Socket? _socket;

    /// <summary>   The input stream used to pull the bytes off the network. </summary>
    private Stream _stream;

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

    /// <summary>Remaining number of bytes in this fragment -- and still to read.</summary>
    private int _fragmentLength;

    /// <summary>
    /// Flag indicating that we've read the last fragment and thus reached
    /// the end of the record.
    /// </summary>
    private bool _lastFragment;

    /// <summary>
    /// Constructs a new <see cref="XdrTcpDecodingStream"/> object and associate it with the given 
    /// <see cref="NetworkStream"/> for TCP/IP-based communication.
    /// </summary>
    /// <param name="socket">       Socket from which XDR data is received. </param>
    /// <param name="bufferSize">   Size of packet buffer for storing received XDR data. </param>
    public XdrTcpDecodingStream( Socket socket, int bufferSize )
    {
        this._socket = socket;
        this._stream = new NetworkStream( socket );

        // If the given buffer size is too small, start with a more sensible
        // size. Next, if bufferSize is not a multiple of four, round it up to
        // the next multiple of four.

        if ( bufferSize < XdrDecodingStreamBase.MinBufferSize ) bufferSize = XdrDecodingStreamBase.MinBufferSize;

        if ( (bufferSize & 3) != 0 )
        {
            bufferSize = (bufferSize + 4) & ~3;
        }

        // Set up the buffer and the buffer pointers.

        this._buffer = new byte[bufferSize];
        this._bufferIndex = 0;
        this._bufferHighmark = -4;
        this._lastFragment = false;
        this._fragmentLength = 0;
    }

    /// <summary>   Returns the Internet address of the sender of the current XDR data. </summary>
    /// <remarks>
    /// This method should only be called after
    /// <see cref="BeginDecoding()"/>, otherwise it might return stale information.
    /// </remarks>
    /// <returns>   <see cref="IPAddress"/> of the sender of the current XDR data. </returns>
    public override IPAddress GetSenderAddress()
    {
        return this._socket== null ? IPAddress.None : ( ( IPEndPoint ) this._socket.RemoteEndPoint ).Address;
    }

    /// <summary>   Returns the port number of the sender of the current XDR data. </summary>
    /// <remarks>
    /// This method should only be called after
    /// <see cref="BeginDecoding()"/>, otherwise it might return stale information.
    /// </remarks>
    /// <returns>   Port number of the sender of the current XDR data. </returns>
    public override int GetSenderPort()
    {
        return this._socket == null ? 0 : (( IPEndPoint ) this._socket.RemoteEndPoint ).Port;
    }

    /// <summary>   Initiates decoding of the next XDR record. </summary>
    /// <remarks>
    /// For TCP-based XDR decoding streams this reads in the next chunk of data from the network 
    /// socket (a chunk of data is not necessary the same as a fragment, just enough to fill the 
    /// internal buffer or receive the remaining part of a fragment). <para>
    /// Read in the next bunch of bytes. This can be either a complete fragment,
    /// or if the fragments sent by the communication partner are too large for our buffer, only
    /// parts of fragments. In every case, this method ensures that there will be more data available
    /// in the buffer (or else an exception thrown). </para>
    /// </remarks>
    ///
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public override void BeginDecoding()
    {
        this.Fill();
    }

    private void ReadBuffer( Stream stream, byte[] bytes, int bytesToRead )
    {
        int bytesRead;
        int byteOffset = 0;
        while ( bytesToRead > 0 )
        {
            bytesRead = stream.Read( bytes, byteOffset, bytesToRead );
            if ( bytesRead <= 0 )
            {

                // Stream is at EOF -- note that bytesRead is not allowed
                // to be zero here, as we asked for at least one byte...

                throw (new XdrException( XdrException.XdrCannotReceive ));
            }
            bytesToRead -= bytesRead;
            byteOffset += bytesRead;
        }
    }

    private void Fill()
    {

        // If the buffer is empty but there are still bytes left to read,
        // refill the buffer. We have also to take care of the record marking
        // within the stream.

        // Remember that lastFragment is reset by the endDecoding() method.
        // This once used to be a while loop, but it has been dropped since
        // we do not accept empty records any more -- with the only exception
        // being a final trailing empty XDR record.

        // Did we already read in all data belonging to the current XDR
        // record, or are there bytes left to be read?

        if ( this._fragmentLength <= 0 )
        {
            if ( this._lastFragment )
            {

                // In case there is no more data in the current XDR record
                // (as we already saw the last fragment), throw an exception.

                throw (new XdrException( XdrException.XdrBufferUnderflow ));
            }

            // First read in the header of the next fragment.

            byte[] bytes = new byte[4];
            this.ReadBuffer( this._stream, bytes, 4 );

            // Watch the sign bit!

            this._fragmentLength = bytes[0] & unchecked(( int ) (0xFF));
            this._fragmentLength = (this._fragmentLength << 8) + (bytes[1] & unchecked(( int ) (0xFF)));
            this._fragmentLength = (this._fragmentLength << 8) + (bytes[2] & unchecked(( int ) (0xFF)));
            this._fragmentLength = (this._fragmentLength << 8) + (bytes[3] & unchecked(( int ) (0xFF)));
            if ( (this._fragmentLength & unchecked(( int ) (0x80000000))) != 0 )
            {
                this._fragmentLength &= unchecked(( int ) (0x7FFFFFFF));
                this._lastFragment = true;
            }
            else
            {
                this._lastFragment = false;
            }

            // Sanity check on incoming fragment length: the length must
            // be at least four bytes long, otherwise this fragment does
            // not make sense. There are ONC/RPC implementations that send
            // empty trailing fragments, so we accept them here.
            // Also check for fragment lengths which are not a multiple of
            // four -- and thus are invalid.

            if ( (this._fragmentLength & 3) != 0 )
            {
                throw (new IOException( "XDR fragment length is not a multiple of four" ));
            }
            if ( (this._fragmentLength == 0) && !this._lastFragment )
            {
                throw (new IOException( "empty XDR fragment which is not a trailing fragment" ));
            }
        }

        // When the reach this stage, there is (still) data to be read for the
        // current XDR record *fragment*.

        // Now read in the next buffer. Depending on how much bytes are still
        // to read within this frame, we either fill the buffer not completely
        // (with still some bytes to read in from the next round) or
        // completely.

        this._bufferIndex = 0;
        if ( this._fragmentLength < this._buffer.Length )
        {
            this.ReadBuffer( this._stream, this._buffer, this._fragmentLength );
            this._bufferHighmark = this._fragmentLength - 4;
            this._fragmentLength = 0;
        }
        else
        {
            this.ReadBuffer( this._stream, this._buffer, this._buffer.Length );
            this._bufferHighmark = this._buffer.Length - 4;
            this._fragmentLength -= this._buffer.Length;
        }
    }

    /// <summary>   End decoding of the current XDR record. </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrDecodingStreamBase.EndDecoding"/> is that calling it is an indication that
    /// the current record is no more interesting to the caller and any allocated data for this 
    /// record can be freed. <para>
    /// This method overrides <see cref="XdrDecodingStreamBase.EndDecoding()"/>.
    /// It reads in and throws away fragments until it reaches the last fragment. </para>
    /// </remarks>
    ///
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public override void EndDecoding()
    {
        try
        {

            // Drain the stream until we reach the end of the current record.

            while ( !this._lastFragment || (this._fragmentLength != 0) )
            {
                this.Fill();
            }
        }
        finally
        {

            // Try to reach a sane state, although this is rather questionable
            // in case of timeouts in the middle of a record.

            this._bufferIndex = 0;
            this._bufferHighmark = -4;
            this._lastFragment = false;
            this._fragmentLength = 0;
        }
    }

    /// <summary>
    /// Closes this decoding XDR stream and releases any system resources associated with this stream.
    /// </summary>
    /// <remarks>
    /// A closed XDR stream cannot perform decoding operations and cannot be reopened. <para>
    /// This implementation frees the allocated buffer but does not close
    /// the associated datagram socket. It only throws away the reference to this socket. </para>
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

    /// <summary>   Decodes (aka "deserializes") a "XDR int" value received from an XDR stream. </summary>
    /// <remarks>
    /// An XDR int encapsulate a 32 bits <see langword="int"/>.
    /// </remarks>
    /// <returns>   The decoded int value. </returns>
    ///
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public override int DecodeInt()
    {

        // This might look funny in the first place, but this way we can
        // properly handle trailing empty XDR record fragments. In this
        // case fill() will return without any now data the first time
        // and on the second time a buffer underflow exception is thrown.

        while ( this._bufferIndex > this._bufferHighmark )
        {
            this.Fill();
        }

        // There's enough space in the buffer to hold at least one
        // XDR int. So let's retrieve it now.
        // Note: buffer[...] gives a byte, which is signed. So if we
        // add it to the value (which is int), it has to be widened
        // to 32 bit, so its sign is propagated. To avoid this sign
        // madness, we have to "and" it with 0xFF, so all unwanted
        // bits are cut off after sign extension. Sigh.

        int value = this._buffer[this._bufferIndex++] & unchecked(( int ) (0xFF));
        value = (value << 8) + (this._buffer[this._bufferIndex++] & unchecked(( int ) (0xFF)));
        value = (value << 8) + (this._buffer[this._bufferIndex++] & unchecked(( int ) (0xFF)));
        value = (value << 8) + (this._buffer[this._bufferIndex++] & unchecked(( int ) (0xFF)));
        return value;
    }

    /// <summary>
    /// Decodes (aka "deserializes") an opaque value, which is nothing more than a series of octets
    /// (or 8 bits wide bytes).
    /// </summary>
    /// <remarks>
    /// Because the length of the opaque value is given, we don't need to
    /// retrieve it from the XDR stream. This is different from
    /// <see cref="DecodeOpaque(byte[], int, int)"/>
    /// where first the length of the opaque value is retrieved from the XDR stream.
    /// </remarks>
    /// <param name="length">   Length of opaque data to decode. </param>
    /// <returns>   Opaque data as a byte vector. </returns>
    ///
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public override byte[] DecodeOpaque( int length )
    {
        int padding = (4 - (length & 3)) & 3;
        int offset = 0;
        // current offset into bytes vector
        int toCopy;

        // Now allocate enough memory to hold the data to be retrieved and
        // get part after part from the buffer.

        byte[] bytes = new byte[length];

        // As for the while loop, see the comment in xdrDecodeInt().

        while ( this._bufferIndex > this._bufferHighmark )
        {
            this.Fill();
        }
        while ( length > 0 )
        {
            toCopy = this._bufferHighmark - this._bufferIndex + 4;
            if ( toCopy >= length )
            {

                // The buffer holds more data than we need. So copy the bytes
                // and leave the stage.

                System.Array.Copy( this._buffer, this._bufferIndex, bytes, offset, length );
                this._bufferIndex += length;
                // No need to adjust "offset", because this is the last round.
                break;
            }
            else
            {

                // We need to copy more data than currently available from our
                // buffer, so we copy all we can get our hands on, then fill
                // the buffer again and repeat this until we got all we want.

                System.Array.Copy( this._buffer, this._bufferIndex, bytes, offset, toCopy );
                this._bufferIndex += toCopy;
                offset += toCopy;
                length -= toCopy;
                // NB: no problems with trailing empty fragments, so we skip
                // the while loop here.
                this.Fill();
            }
        }
        this._bufferIndex += padding;
        return bytes;
    }

    /// <summary>
    /// Decodes (aka "deserializes") a XDR opaque value, which is represented by a vector of byte
    /// values, and starts at <paramref name="offset"/> with a length of <paramref name="length"/>.
    /// </summary>
    /// <remarks>
    /// Only the opaque value is decoded, so the caller has to know how long the opaque value will be. The
    /// decoded data is always padded to be a multiple of four (because that's what the sender does).
    /// </remarks>
    /// <param name="opaque">   Byte vector which will receive the decoded opaque value. </param>
    /// <param name="offset">   Start offset in the byte vector. </param>
    /// <param name="length">   the number of bytes to decode. </param>
    ///
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="IOException">   Thrown when an I/O error condition occurs. </exception>
    public override void DecodeOpaque( byte[] opaque, int offset, int length )
    {
        int padding = (4 - (length & 3)) & 3;
        int toCopy;

        // Now get part after part and fill the byte vector.

        if ( this._bufferIndex > this._bufferHighmark )
        {
            this.Fill();
        }
        while ( length > 0 )
        {
            toCopy = this._bufferHighmark - this._bufferIndex + 4;
            if ( toCopy >= length )
            {

                // The buffer holds more data than we need. So copy the bytes
                // and leave the stage.

                System.Array.Copy( this._buffer, this._bufferIndex, opaque, offset, length );
                this._bufferIndex += length;
                // No need to adjust "offset", because this is the last round.
                break;
            }
            else
            {

                // We need to copy more data than currently available from our
                // buffer, so we copy all we can get our hands on, then fill
                // the buffer again and repeat this until we got all we want.

                System.Array.Copy( this._buffer, this._bufferIndex, opaque, offset, toCopy );
                this._bufferIndex += toCopy;
                offset += toCopy;
                length -= toCopy;
                this.Fill();
            }
        }
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
    ~XdrTcpDecodingStream()
    {
        if ( this.IsDisposed ) { return; }
        this.Dispose( false );
    }

    #endregion

}
