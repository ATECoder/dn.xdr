namespace cc.isr.XDR.Codecs;

/// <summary>
/// The abstract base class <see cref="UnionXdrCodecBase"/> <see cref="UnionXdrCodecBase"/> for
/// the serialization of polymorphic classes.
/// </summary>
/// <remarks>
/// This class should not be confused with C unions in general. Instead <see cref="UnionXdrCodecBase"/>
/// is an object-oriented Constructs which helps in deploying polymorphism. <para>
/// TO_DO: Add unit tests for this class. </para> <para>
/// The original Remote Tea links to code samples are no longer accessible. </para> <para>
/// The serialization scheme implemented by <see cref="UnionXdrCodecBase"/> is only a question of
/// getting used to it: after serializing the type code of the polymorphic class, the variant
/// part is serialized first before the common part. This behavior stems from the ACPLT C++
/// Communication Library and has been retained for compatibility reasons. As it doesn't hurt,
/// you need not mind anyway. </para> <para>
/// To use polymorphism with XDR streams, you'll have to derive your own base class (let's call
/// it <c>Foo</c>) from <see cref="UnionXdrCodecBase"/> and implement the two methods
/// <see cref="XdrEncodeCommon(XdrEncodingStreamBase)"/>
/// and
/// <see cref="XdrDecodeCommon(XdrDecodingStreamBase)"/>.
/// Do not overwrite the methods <see cref="Encode(XdrEncodingStreamBase)"/>
/// and <see cref="Decode(XdrDecodingStreamBase)"/>!  </para> <para>
/// Then, in your <c>Foo</c>-derived classes, like <c>Bar</c> and <c>FooBar</c>, implement the
/// other two methods <see cref="XdrEncodeVariant(XdrEncodingStreamBase)"/> and
/// <see cref="XdrDecodeVariant(XdrDecodingStreamBase)"/>.
/// In addition, set <see cref="XdrTypeCode"/> when instantiating the derived class to set an <see cref="int"/>
/// , unique identifier of your class. Note that this identifier only needs to be unique within
/// the scope of your <c>Foo</c> class. </para><para>
/// Remote Tea authors: Harald Albrecht, Jay Walters.</para>
/// </remarks>
public abstract class UnionXdrCodecBase : IXdrCodec
{

    /// <summary>   Specialized constructor for use only by derived class. </summary>
    /// <param name="xdrTypeCode">  Type code identifying an object's class when encoding or decoding
    ///                             the object into or from a XDR stream. </param>
    protected UnionXdrCodecBase( int xdrTypeCode )
    {
        this.XdrTypeCode = xdrTypeCode;
    }

    /// <summary>
    /// Gets or set (private) the so-called type code which identifies a derived class when encoded or decoded.
    /// </summary>
    /// <remarks>
    /// Note that the type code is not globally unique, but rather it is only unique within the
    /// derived classes of a direct descend of <see cref="UnionXdrCodecBase"/>. If <c>Foo</c> is derived
    /// from <see cref="UnionXdrCodecBase"/> and <c>Foo</c> is the base class for <c>Bar</c> and
    /// <c>FooBar</c>, then the type code needs only be unique between <c>Bar</c> and <c>FooBar</c>.
    /// </remarks>
    /// <value>
    /// Type code identifying an object's class when encoding or decoding the object into or from a
    /// XDR stream.
    /// </value>
    public int XdrTypeCode { get; private set; }

    /// <summary>
    /// Encodes -- that is: serializes -- an object into an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {

        // For historical reasons (read: "for dumb and pure idiotic reasons")
        // and compatibility with the ACPLT/KS C++ Communication Library we
        // encode/decode the variant part *first* before encoding/decoding
        // the common part.

        encoder.EncodeInt( this.XdrTypeCode );
        this.XdrEncodeVariant( encoder );
        this.XdrEncodeCommon( encoder );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- an object from an XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    ///
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {

        // Make sure that when deserializing this object's state that
        // the stream provides state information indeed intended for this
        // particular class.

        int xdrTypeCode = decoder.DecodeInt();
        if ( xdrTypeCode != this.XdrTypeCode )
            throw new Exception( $"{this.GetType().Name} non-matching XDR type code received." );

        // For historical reasons (read: "for dumb and pure idiotic reasons")
        // and compatibility with the ACPLT/KS C++ Communication Library we
        // encode/decode the variant part *first* before encoding/decoding
        // the common part.

        this.XdrDecodeVariant( decoder );
        this.XdrDecodeCommon( decoder );
    }

    /// <summary>
    /// Encodes -- that is: serializes -- the common part of an object into an XDR stream in
    /// compliance to RFC 1832.
    /// </summary>
    /// <remarks>
    /// Note that the common part is deserialized after the variant part.
    /// </remarks>
    /// <param name="encoder">  The XDR encoding stream. </param>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public abstract void XdrEncodeCommon( XdrEncodingStreamBase encoder );

    /// <summary>
    /// Decodes -- that is: deserializes -- the common part of an object from an XDR stream in
    /// compliance to RFC 1832.
    /// </summary>
    /// <remarks>
    /// Note that the common part is deserialized after the variant part.
    /// </remarks>
    /// <param name="decoder">  The XDR encoding stream. </param>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    public abstract void XdrDecodeCommon( XdrDecodingStreamBase decoder );

    /// <summary>
    /// Encodes -- that is: serializes -- the variant part of an object into an XDR stream in
    /// compliance to RFC 1832.
    /// </summary>
    /// <remarks>
    /// Note that the variant part is deserialized before the common part.
    /// </remarks>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    /// <param name="encoder">  The XDR encoding stream. </param>
    public abstract void XdrEncodeVariant( XdrEncodingStreamBase encoder );

    /// <summary>
    /// Decodes -- that is: deserializes -- the variant part of an object from an XDR stream in
    /// compliance to RFC 1832.
    /// </summary>
    /// <remarks>
    /// Note that the variant part is deserialized before the common part.
    /// </remarks>
    /// <exception cref="XdrException">             Thrown when an XDR error condition occurs. </exception>
    /// <exception cref="System.IO.IOException">    Thrown when an I/O error condition occurs. </exception>
    /// <param name="decoder">  The XDR encoding stream. </param>
    public abstract void XdrDecodeVariant( XdrDecodingStreamBase decoder );
}