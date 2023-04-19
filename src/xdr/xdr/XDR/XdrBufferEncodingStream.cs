
using System.Collections.Generic;
using System.Linq;

namespace cc.isr.XDR;

/// <summary>
/// The <see cref="XdrBufferEncodingStream"/> class provides a buffer-based XDR stream.
/// </summary>
/// <remarks> <para>
/// 
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public class XdrBufferEncodingStream : XdrEncodingStreamBase
{

    #region " Construction and Cleanup "

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
    ~XdrBufferEncodingStream()
    {
        if ( this.IsDisposed ) { return; }
        this.Dispose( false );
    }


    #endregion

    #region " members "

    /// <summary>   Returns the amount of encoded data in the buffer. </summary>
    /// <returns>   length of data encoded in buffer. </returns>
    public virtual int GetEncodedDataLength()
    {
        return this._bufferIndex;
    }

    /// <summary>   Returns the buffer holding encoded data. </summary>
    /// <returns>   Buffer with encoded data. </returns>
    public virtual byte[] GetEncodedData()
    {
        return this._buffer;
    }

    #endregion

    #region " actions "

    /// <summary>   Begins encoding a new XDR record. </summary>
    /// <remarks>   This involves resetting this encoding XDR stream back into a known state. </remarks>
    public void BeginEncoding()
    {
        this._bufferIndex = 0;
    }

    /// <summary>   Begins encoding a new XDR record. </summary>
    /// <remarks>   This involves resetting this encoding XDR stream back into a known state. </remarks>
    /// <param name="remoteEndPoint">   Indicates the remote end point of the receiver of the XDR
    ///                                 data. This can be (<see langword="null"/>) for XDR streams
    ///                                 connected permanently to a receiver (like in case of TCP/IP
    ///                                 based XDR streams). </param>
    public override void BeginEncoding( IPEndPoint remoteEndPoint )
    {
        this.BeginEncoding();
    }

    /// <summary>
    /// Flushes this encoding XDR stream and forces any buffered output bytes to be written out.
    /// </summary>
    /// <remarks>
    /// The general contract of <see cref="XdrEncodingStreamBase.EndEncoding()"/> is that calling it is
    /// an indication that the current record is finished and any bytes previously encoded should 
    /// immediately be written to their intended destination.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    public override void EndEncoding()
    {
    }

    #endregion

    #region " encode actions "

    /// <summary>
    /// Encodes (aka "serializes") an XDR integer value into an XDR stream.
    /// </summary>
    /// <remarks>
    /// An XDR int encapsulate a 32 bits <see cref="int"/>.
    /// This method is one of the basic methods all other methods can rely on.
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The int value to be encoded. </param>
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
    /// Encodes (aka "serializes") a XDR opaque value, which is represented by a vector of <see cref="byte"/>
    /// values, and starts at <paramref name="offset"/> with a length of <paramref name="length"/>.
    /// </summary>
    /// <remarks>
    /// This just copies the input values, padded with zeros to a length that is multiple of 4,
    /// into the internal buffer starting at the current buffer index. <para> 
    /// 
    /// Only the opaque value is encoded, but no length indication is preceding the opaque value, so the
    /// receiver has to know how long the opaque value will be. The encoded data is always padded to
    /// be a multiple of four. If the given length is not a multiple of four, zero bytes will be used
    /// for padding. </para>
    /// </remarks>
    /// <exception cref="XdrException">  Thrown when an XDR error condition occurs. </exception>
    /// <param name="value">    The opaque value to be encoded in the form of a series of bytes. </param>
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

                // If the length of the opaque data was not a multiple of 4 bytes, then
                // pad with zeros, so the next write pointer points to a byte, which index
                // is a multiple of four.

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
