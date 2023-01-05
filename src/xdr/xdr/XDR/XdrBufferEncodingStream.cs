
namespace cc.isr.XDR;

/// <summary>
/// The <see cref="XdrBufferEncodingStream"/> class provides a buffer-based XDR stream.
/// </summary>
/// <remarks> <para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class XdrBufferEncodingStream : XdrEncodingStreamBase
{

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
    /// Constructs a new <see cref="XdrBufferEncodingStream"/> with a buffer to encode data into of
    /// the given size.
    /// </summary>
    /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
    ///                                         illegal values. </exception>
    /// <param name="bufferSize">   Size of buffer to store encoded data in. </param>
    public XdrBufferEncodingStream( int bufferSize )
    {
        if ( (bufferSize < 0) || (bufferSize & 3) != 0 )
        {
            throw (new ArgumentException( "size of buffer must be a multiple of four and must not be negative" ));
        }
        this._buffer = new byte[bufferSize];
        this._bufferIndex = 0;
        this._bufferHighmark = this._buffer.Length - 4;
    }

    /// <summary>   Constructs a new <see cref="XdrBufferEncodingStream"/> with a given buffer. </summary>
    /// <exception cref="ArgumentException">    if <paramref name="buffer"/> length is not a multiple of
    ///                                         four. </exception>
    /// <param name="buffer">   Buffer to store encoded information in. </param>
    public XdrBufferEncodingStream( byte[] buffer )
    {

        // Make sure that the buffer size is a multiple of four, otherwise
        // throw an exception.

        if ( (buffer.Length & 3) != 0 )
        {
            throw (new ArgumentException( "size of buffer must be a multiple of four" ));
        }
        this._buffer = buffer;
        this._bufferIndex = 0;
        this._bufferHighmark = buffer.Length - 4;
    }

    /// <summary>   Returns the amount of encoded data in the buffer. </summary>
    /// <returns>   length of data encoded in buffer. </returns>
    public virtual int GetXdrLength()
    {
        return this._bufferIndex;
    }

    /// <summary>   Returns the buffer holding encoded data. </summary>
    /// <returns>   Buffer with encoded data. </returns>
    public virtual byte[] GetXdrData()
    {
        return this._buffer;
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
        this._bufferIndex = 0;
    }

    /// <summary>
    /// Flushes this encoding XDR stream and forces any buffered output bytes to be written out.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrEncodingStreamBase.EndEncoding()"/> is that calling it is
    /// an indication that the current record is finished and any bytes previously encoded should 
    /// immediately be written to their intended destination.
    /// </remarks>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public override void EndEncoding()
    {
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
    }

    /// <summary>
    /// Encodes (aka "serializes") a "XDR int" value and writes it down an XDR stream.
    /// </summary>
    /// <remarks>
    /// An XDR int encapsulate a 32 bits <see langword="int"/>.
    /// This method is one of the basic methods all other methods can rely on.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
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
    /// Only the opaque value is encoded, but no length indication is preceding the opaque value, so the
    /// receiver has to know how long the opaque value will be. The encoded data is always padded to
    /// be a multiple of four. If the given length is not a multiple of four, zero bytes will be used
    /// for padding.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
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

                // If the length of the opaque data was not a multiple of 4 bytes, then
                // pad with zeros, so the next write pointer points to a byte, which index
                // is a multiple of four.

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
            // set large fields to null
            this._buffer = null;
        }
        finally
        {
            base.Dispose( disposing );
        }
    }

    #endregion

}
