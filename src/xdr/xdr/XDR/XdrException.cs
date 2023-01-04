
namespace cc.isr.XDR;

/// <summary>
/// The class <see cref="XdrException"/> indicates XDR conditions that a reasonable
/// application might want to catch.
/// </summary>
[Serializable]
public class XdrException : Exception
{

    /// <summary>
    /// Constructs an <see cref="XdrException"/> with a reason of <see cref="XdrFailed"/>.
    /// </summary>
    public XdrException() : this( XdrException.XdrFailed )
    {
    }

    /// <summary>
    /// Constructs an <see cref="XdrException"/> with the specified detail message.
    /// </summary>
    /// <param name="message"> The detail message. </param>
    public XdrException( string message ) : base()
    {
        this.Reason = XdrFailed;
        this._message = message;
    }

    /// <summary>
    /// Constructs an <see cref="XdrException"/> with the specified detail reason and message.
    /// </summary>
    /// <param name="reason">  The detail reason. </param>
    /// <param name="message"> The detail message. </param>
    public XdrException( int reason, string message ) : base()
    {
        this.Reason = reason;
        this._message = message;
    }

    /// <summary>
    /// Constructs an <see cref="XdrException"/> with the specified detail reason.
    /// </summary>
    /// <remarks>   The detail message is derived automatically from the <paramref name="reason"/>. </remarks>
    /// <param name="reason">   The reason. This can be one of the constants -- oops, that should be
    ///                         "public final static integers" -- defined in this interface. </param>
    public XdrException( int reason ) : base()
    {
        this.Reason = reason;
        switch ( reason )
        {
            case XdrCannotReceive:
                {
                    this._message = "cannot receive XDR data";
                    break;
                }

            case XdrCannotSend:
                {
                    this._message = "cannot send XDR data";
                    break;
                }

            case XdrFailed:
                {
                    this._message = "XDR generic failure";
                    break;
                }

            case XdrBufferOverflow:
                {
                    this._message = "XDR buffer overflow";
                    break;
                }

            case XdrBufferUnderflow:
                {
                    this._message = "XDR buffer underflow";
                    break;
                }

            case XdrSuccess:
            default:
                {
                    break;
                }
        }
    }

    /// <summary>
    /// Specific detail (reason) about this <see cref="XdrException"/>, like the XDR error code, as defined
    /// by the <c>Xdr</c> constants of this class.
    /// </summary>
    /// <value> The reason for this <see cref="XdrException"/> if it was
    /// <see cref="XdrException(int)">created</see> with an error reason; or
    /// <see cref="XdrFailed"/> if it was <see cref="XdrException()">created</see>
    /// with no error reason (using the default constructor).
    /// </value>
    /// 
    public int Reason { get; private set; }

    /// <summary>
    /// Specific detail about this <see cref="XdrException"/>, like a detailed error message.
    /// </summary>
    private readonly string _message;

    /// <summary>   Returns the error message string of this XDR object. </summary>
    /// <value>
    /// The error message string of this <see cref="XdrException"/>
    /// object if it was created either with an error message string or an XDR error code.
    /// </value>
    public override string Message  => this._message;

    /// <summary>   (Immutable) The remote procedure call was carried out successfully. <para>
    /// Stands for Remote Tea RPC_SUCCESS (=0), which maps to VXI-11 Visa32.VISA.VI_SUCCESS </para></summary>
    public const int XdrSuccess = 0;

    /// <summary>   (Immutable) Encoded information cannot be sent. <para>
    /// Stands for Remote Tea RPC_CANTSEMD (=3)  </para></summary>
    public const int XdrCannotSend = 1;

    /// <summary>
    /// (Immutable) Information to be decoded cannot be received. <para> 
    /// Stands for Remote Tea RPC_CANTRECV (=4) </para></summary>
    public const int XdrCannotReceive = 2;

    /// <summary>   (Immutable) A generic XDR exception occurred. <para>
    /// Stands for Remote Tea RPC_FAILED (=16)  </para></summary>
    public const int XdrFailed = 3;

    /// <summary>
    /// (Immutable) A buffer overflow occurred with an encoding XDR stream. This happens if you use
    /// UDP-based (datagram-based) XDR streams and you try to encode more data than can fit into the
    /// sending buffers. <para>
    /// Stands for Remote Tea RPC_BUFFEROVERFLOW (=42)  </para></summary>
    public const int XdrBufferOverflow = 4;

    /// <summary>
    /// (Immutable) A buffer underflow occurred with an decoding XDR stream. This happens if you try
    /// to decode more data than was sent by the other communication partner. <para>
    /// Stands for Remote Tea RPC_BUFFERUNDERFLOW (=43)  </para></summary>
    public const int XdrBufferUnderflow = 5;

}

