# eXternal Data Representation (XDR)

External Data Representation ([XDR]) is a standard data serialization format for uses such as computer network protocols. It allows data to be transferred between different kinds of computer systems. Converting from the local representation to XDR is called encoding. Converting from XDR to the local representation is called decoding. XDR is implemented as a software library of functions which is portable between different operating systems and is also independent of the transport layer.

XDR uses a base unit of 4 bytes, serialized in big-endian order; smaller data types still occupy four bytes each after encoding. Variable-length types such as string and opaque are padded to a total divisible by four bytes. Floating-point numbers are represented in IEEE 754 format.

## Opaque Data


## Opaque Data

### Fixed-Length Opaque Data

At times, fixed-length uninterpreted data needs to be passed among machines.  This data is called "opaque" and is declared as follows:
```
    byte[] opaque = new byte[n];
```
where the constant n is the (static) number of bytes necessary to contain the opaque data. If n is not a multiple of four, then the n bytes are followed by enough (0 to 3) residual zero bytes, r, to make the total byte count of the opaque object a multiple of four.

```
           0        1    ...
      +--------+--------+...+--------+--------+...+--------+
      | byte 0 | byte 1 |...|byte n-1|    0   |...|    0   |
      +--------+--------+...+--------+--------+...+--------+
      |<-----------n bytes---------->|<------r bytes------>|
      |<-----------n+r (where (n+r) mod 4 = 0)------------>|
```
<a name="Figure1"></a>
#### Figure 1. Fixed-Length Opaque. 
This diagram contains 4 lines of information. The second line of the diagram is the main line, listing bytes as follows: byte 0, byte 1, dots signifying the bytes between byte 1 and byte n-1. The next byte is labeled: byte n-1, and is followed by residual byte 0. Dots signify more residual bytes that end in a final byte 0. The remaining lines of the diagram describe this main line of bytes. The first line assigns numbers to the bytes as follows: number 0 for byte 0, number 1 for byte 1, and dots signifying a continuing sequence. The third line assigns <see cref="byte"/> values to the bytes in the main line as follows: byte 0 through byte n-1 yield n bytes. All the residual bytes together equal r bytes. The fourth line, which spans the entire diagram, shows the following equation:n+r (where (n+r) mod 4 = 0).


### Variable-Length Opaque Data
XDR also defines variable-length uninterpreted data as opaque. Variable-length (counted) opaque data is defined as a sequence of n arbitrary bytes, numbered 0 through n-1. Opaque data is encoded as an unsigned integer of value equals n followed by the n bytes of the sequence.

Byte m of the sequence always precedes byte m+1, and byte 0 of the sequence always follows the sequence length (count). Enough (0 to 3) residual 0 bytes, r, are added to make the total byte count a multiple of 4 ([Figure 2](#Figure2).

Variable-length opaque data is declared in one of the following forms:
```
         byte[] opaque = new byte[m];
```
The constant m denotes an upper bound of the number of bytes that the sequence may contain. The constant m would normally be found in a protocol specification. For example, VXI-11 may state that the maximum data transfer size is 8192 bytes, as follows:

```
         byte[] buffer = new byte[8192];
```
```
            0     1     2     3     4     5   ...
         +-----+-----+-----+-----+-----+-----+...+-----+-----+...+-----+
         |        length n       |byte0|byte1|...| n-1 |  0  |...|  0  |
         +-----+-----+-----+-----+-----+-----+...+-----+-----+...+-----+
         |<-------4 bytes------->|<------n bytes------>|<---r bytes--->|
                                 |<----n+r (where (n+r) mod 4 = 0)---->|
```
<a name="Figure2"></a>
#### Figure 2. Variable-Length Opaque. 
This form is similar to the [Fixed-Length Opaque](#Figure1) except for the starting 4 bytes for the data size `n`. 

## References
[XDR]  
[IBM XDR]

[XDR]: https://www.rfc-editor.org/rfc/rfc4506#section-4.7
[IBM XDR]: https://www.ibm.com/docs/en/aix/7.1?topic=concepts-external-data-representation
