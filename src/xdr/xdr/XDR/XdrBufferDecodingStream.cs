
namespace cc.isr.XDR;

/// <summary>
/// The <see cref="XdrBufferDecodingStream"/> class provides the necessary functionality to retrieve XDR
/// packets from a byte buffer.
/// </summary>
/// <remarks>
/// Remote Tea authors: Harald Albrecht, Jay Walters.
/// </remarks>
public class XdrBufferDecodingStream : XdrDecodingStreamBase
{
    #region " construction and cleanup "

    /// <summary>
    /// The buffer which will be filled from the datagram socket and then
    /// be used to supply the information when decoding data.
    /// </summary>
    private byte[] _buffer;

    /// <summary>Length of encoded data in <see cref="_buffer"/>.</summary>
    private int _encodedLength;

    /// <summary>The read pointer is an index into the <see cref="_buffer"/>.</summary>
    private int _bufferIndex;

    /// <summary>
    /// Index of the last four byte word in the buffer, which has been read
    /// in from the datagram socket.
    /// </summary>
    private int _bufferHighmark;

    /// <summary>
    /// Constructs a new <see cref="XdrUdpDecodingStream"/> object and associate it with a buffer
    /// containing encoded XDR data.
    /// </summary>
    /// <param name="buffer">           Buffer containing encoded XDR data. </param>
    /// <param name="encodedLength">    Length of encoded XDR data within the buffer. </param>
    ///
    /// <exception cref="ArgumentException">    if <paramref name="encodedLength"/> is not a multiple
    ///                                         of four. </exception>
    public XdrBufferDecodingStream( byte[] buffer, int encodedLength )
    {
        // Make sure that the buffer size is a multiple of four, otherwise
        // throw an exception.
        if ( (encodedLength < 0) || (encodedLength & 3) != 0 )
        {
            throw (new ArgumentException( "length of encoded data must be a multiple of four and must not be negative" ));
        }
        this._buffer = buffer;
        this._encodedLength = encodedLength;
        this._bufferIndex = 0;
        this._bufferHighmark = -4;
    }

    /// <summary>
    /// Constructs a new <see cref="XdrUdpDecodingStream"/> object and associate it with a buffer
    /// containing encoded XDR data.
    /// </summary>
    /// <param name="buffer">   Buffer containing encoded XDR data. </param>
    ///
    /// <exception cref="ArgumentException">    if the size of the buffer is not a multiple of
    ///                                         four. </exception>
    public XdrBufferDecodingStream( byte[] buffer ) : this( buffer, buffer.Length )
    {
    }

    /// <summary>
    /// Closes this decoding XDR stream and releases any system resources associated with this stream.
    /// </summary>
    /// <remarks>
    /// A closed XDR stream cannot perform decoding operations and cannot be reopened.
    /// This implementation frees the allocated buffer.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public override void Close()
    {
        base.Close();
        this._buffer = Array.Empty<byte>();
    }

    #endregion

    #region " members "

    /// <summary>
    /// Sets the buffer containing encoded XDR data as well as the length of the encoded data.
    /// </summary>
    /// <exception cref="ArgumentException">    if <paramref name="encodedLength"/> is not a multiple of
    ///                                         four. </exception>
    /// <param name="buffer">           Buffer containing encoded XDR data. </param>
    /// <param name="encodedLength">    Length of encoded XDR data within the buffer. </param>
    public virtual void SetEncodedData( byte[] buffer, int encodedLength )
    {

        // Make sure that the buffer size is a multiple of four, otherwise
        // throw an exception.

        if ( (encodedLength < 0) || (encodedLength & 3) != 0 )
        {
            throw (new ArgumentException( "length of encoded data must be a multiple of four and must not be negative" ));
        }
        this._buffer = buffer;
        this._encodedLength = encodedLength;
        this._bufferIndex = 0;
        this._bufferHighmark = -4;
    }

    /// <summary>   Gets the Internet address of the sender of the current XDR data. </summary>
    /// <remarks>
    /// This value is valid only after <see cref="BeginDecoding()"/>, otherwise it might return stale
    /// information.
    /// </remarks>
    /// <value> <see cref="IPAddress"/> of the sender of the current XDR data. </value>
    public override IPAddress? SenderAddress { get { return null; } } 

    /// <summary>   Gets the port number of the sender of the current XDR data. </summary>
    /// <remarks>
    /// This value is valid only after <see cref="BeginDecoding()"/>, otherwise it might return stale
    /// information.
    /// </remarks>
    /// <value> Port number of the sender of the current XDR data. </value>
    public override int SenderPort { get { return 0; } }

    #endregion

    #region " actions "


    /// <summary>   Initiates decoding of the next XDR record. </summary>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public override void BeginDecoding()
    {
        this._bufferIndex = 0;
        this._bufferHighmark = this._encodedLength - 4;
    }

    /// <summary>   End decoding of the current XDR record. </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrDecodingStreamBase.EndDecoding"/> is that calling it is an indication that
    /// the current record is no more interesting to the caller and any allocated data for this
    /// record can be freed. <para>
    /// This method overrides <see cref="XdrDecodingStreamBase.EndDecoding()"/>.
    /// It does nothing more than resetting the buffer pointer back
    /// to the begin of an empty buffer, so attempts to decode data will fail
    /// until the buffer is filled again.</para>
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public override void EndDecoding()
    {
        this._bufferIndex = 0;
        this._bufferHighmark = -4;
    }

    #endregion

    #region " decode actions "

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
    /// Because the length of the opaque value is given, we don't need to
    /// retrieve it from the XDR stream. This is different from
    /// <see cref="DecodeOpaque(byte[], int, int)"/>
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
    /// Decodes (aka "deserializes") a XDR opaque value, which is represented by a vector of byte
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
