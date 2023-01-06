#nullable disable


namespace cc.isr.XDR.MSTest.Codecs;


/// <summary>   (Serializable) a binary tree 2 XBR encoder/decoder. </summary>
/// <remarks>   2022-12-30. </remarks>
[Serializable]
public class BinaryTree2Codec : IXdrCodec
{
    /// <summary>   Default constructor. </summary>
    /// <remarks>   2022-12-22. </remarks>
    public BinaryTree2Codec()
    {
    }

    /// <summary>   Constructor. </summary>
    /// <remarks>   2022-12-22. </remarks>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public BinaryTree2Codec( XdrDecodingStreamBase decoder )
    {
        this.Decode( decoder );
    }

    /// <summary>   Gets or sets the key. </summary>
    /// <value> The key. </value>
    public string Key { get; set; }

    /// <summary>   Gets or sets the value. </summary>
    /// <value> The value. </value>
    public string Value { get; set; }

    /// <summary>   Gets or sets the left. </summary>
    /// <value> The left. </value>
    public BinaryTree2Codec Left { get; set; }

    /// <summary>   Gets or sets the right. </summary>
    /// <value> The right. </value>
    public BinaryTree2Codec Right { get; set; }

    /// <summary>
    /// Encodes -- that is: serializes -- an object into a XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <remarks>
    /// Encodes -- that is: serializes -- an object into a XDR stream in compliance to RFC 1832.
    /// </remarks>
    /// <param name="encoder">  XDR stream to which information is sent for encoding. </param>
    public virtual void Encode( XdrEncodingStreamBase encoder )
    {
        BinaryTree2Codec currentBinaryTree = this;
        do
        {
            encoder.EncodeString( currentBinaryTree.Key );
            encoder.EncodeString( currentBinaryTree.Value );
            if ( currentBinaryTree.Left != null )
            {
                encoder.EcodeBoolean( true );
                currentBinaryTree.Left.Encode( encoder );
            }
            else
                encoder.EcodeBoolean( false );
;
            currentBinaryTree = currentBinaryTree.Right;
            encoder.EcodeBoolean( currentBinaryTree != null );
        } while ( currentBinaryTree != null );
    }

    /// <summary>
    /// Decodes -- that is: deserializes -- an object from a XDR stream in compliance to RFC 1832.
    /// </summary>
    /// <remarks>
    /// Decodes -- that is: deserializes -- an object from a XDR stream in compliance to RFC 1832.
    /// </remarks>
    /// <param name="decoder">  XDR stream from which decoded information is retrieved. </param>
    public virtual void Decode( XdrDecodingStreamBase decoder )
    {
        BinaryTree2Codec currentBinaryTree = this;
        BinaryTree2Codec nextBinaryTree;
        do
        {
            currentBinaryTree.Key = decoder.DecodeString();
            currentBinaryTree.Value = decoder.DecodeString();
            currentBinaryTree.Left = decoder.DecodeBoolean() ? new BinaryTree2Codec( decoder ) : null;
            nextBinaryTree = decoder.DecodeBoolean() ? new BinaryTree2Codec() : null;
            currentBinaryTree.Right = nextBinaryTree;
            currentBinaryTree = nextBinaryTree;
        } while ( currentBinaryTree != null );
    }

}