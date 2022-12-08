namespace AdventOfCode.Utils;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

public class MD5Managed : HashAlgorithm
{
    private const int S11 = 7;
    private const int S12 = 12;
    private const int S13 = 17;
    private const int S14 = 22;
    private const int S21 = 5;
    private const int S22 = 9;
    private const int S23 = 14;
    private const int S24 = 20;
    private const int S31 = 4;
    private const int S32 = 11;
    private const int S33 = 16;
    private const int S34 = 23;
    private const int S41 = 6;
    private const int S42 = 10;
    private const int S43 = 15;
    private const int S44 = 21;

    private static readonly byte[] PADDING;
    private const int BlockSize = 64;
    public readonly uint[] fState = new uint[4];
    public readonly byte[] fBuffer = new byte[BlockSize];
    public ulong fCount;


    static MD5Managed()
    {
        PADDING = new byte[64];
        PADDING[0] = 0x80;
    }

    public MD5Managed()
    {
        HashSizeValue = 128;
        InitializeVariables();
    }

    public override void Initialize()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        fCount = 0;

        fState[0] = 0x67452301;
        fState[1] = 0xefcdab89;
        fState[2] = 0x98badcfe;
        fState[3] = 0x10325476;
    }

    protected override void HashCore(byte[] array, int ibStart, int cbSize)
        => HashCore(array.AsSpan(ibStart, cbSize));

    protected override byte[] HashFinal()
    {
        var buf = new byte[HashSizeValue / 8];
        MD5Final(buf);
        return buf;
    }

    protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten)
    {
        MD5Final(destination);
        bytesWritten = HashSizeValue / 8;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ROTATE_LEFT(uint x, int n) => (x << n) | (x >> (32 - n));

    /* FF, GG, HH, and II transformations for rounds 1, 2, 3, and 4.
       Rotation is separate from addition to prevent recomputation. */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint FF(uint a, uint b, uint c, uint d, uint x, int s, uint ac)
    {
        a += (b & c) | ((~b) & d);
        a += x; 
        a += ac;
        return ROTATE_LEFT(a, s) + b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint GG(uint a, uint b, uint c, uint d, uint x, int s, uint ac)
    {
        a += (b & d) | (c & (~d));
        a += x;
        a += ac;
        return ROTATE_LEFT((a), (s)) + b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint HH(uint a, uint b, uint c, uint d, uint x, int s, uint ac)
    {
        a += (b ^ c ^ d);
        a += x;
        a += ac;
        return ROTATE_LEFT((a), (s)) + b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint II(uint a, uint b, uint c, uint d, uint x, int s, uint ac)
    {
        a += c ^ (b | (~d));
        a += x;
        a += ac;
        return ROTATE_LEFT(a, s) + b;
    }

    protected override void HashCore(ReadOnlySpan<byte> input)
    {
        int index = (int)(fCount & 0x3F);
        uint inputLen = (uint)input.Length;
     
        fCount += inputLen;
        
        int partLen = BlockSize - index;

        int i = 0;
        if (inputLen >= partLen)
        {
            input.Slice(0, partLen).CopyTo(fBuffer.AsSpan(index));
            
            MD5Transform(fBuffer);

            for (i = partLen; i + BlockSize <= inputLen; i += BlockSize)
                MD5Transform(input.Slice(i));

            index = 0;
        }

        input.Slice(i).CopyTo(fBuffer.AsSpan(index));
    }

    private void MD5Final(Span<byte> digest)
    {
        Span<byte> bits = stackalloc byte[8];

        ulong cnt = fCount << 3;
        MemoryMarshal.Write(bits, ref cnt);

        int index = (int)(fCount & 0x3f);
        int padLen = (index < 56) ? (56 - index) : (120 - index);

        HashCore(PADDING.AsSpan(0, padLen));
        HashCore(bits);
        MemoryMarshal.AsBytes(fState.AsSpan()).CopyTo(digest);
    }

    private void MD5Transform(ReadOnlySpan<byte> block)
    {
        uint a = fState[0], b = fState[1], c = fState[2], d = fState[3];
        Span<uint> x = stackalloc uint[16];

        block.Slice(0, BlockSize).CopyTo(MemoryMarshal.AsBytes(x));

        /* Round 1 */
        a = FF(a, b, c, d, x[0], S11, 0xd76aa478); /* 1 */
        d = FF(d, a, b, c, x[1], S12, 0xe8c7b756); /* 2 */
        c = FF(c, d, a, b, x[2], S13, 0x242070db); /* 3 */
        b = FF(b, c, d, a, x[3], S14, 0xc1bdceee); /* 4 */
        a = FF(a, b, c, d, x[4], S11, 0xf57c0faf); /* 5 */
        d = FF(d, a, b, c, x[5], S12, 0x4787c62a); /* 6 */
        c = FF(c, d, a, b, x[6], S13, 0xa8304613); /* 7 */
        b = FF(b, c, d, a, x[7], S14, 0xfd469501); /* 8 */
        a = FF(a, b, c, d, x[8], S11, 0x698098d8); /* 9 */
        d = FF(d, a, b, c, x[9], S12, 0x8b44f7af); /* 10 */
        c = FF(c, d, a, b, x[10], S13, 0xffff5bb1); /* 11 */
        b = FF(b, c, d, a, x[11], S14, 0x895cd7be); /* 12 */
        a = FF(a, b, c, d, x[12], S11, 0x6b901122); /* 13 */
        d = FF(d, a, b, c, x[13], S12, 0xfd987193); /* 14 */
        c = FF(c, d, a, b, x[14], S13, 0xa679438e); /* 15 */
        b = FF(b, c, d, a, x[15], S14, 0x49b40821); /* 16 */

        /* Round 2 */
        a= GG(a, b, c, d, x[1], S21, 0xf61e2562);   /* 17 */
        d = GG(d, a, b, c, x[6], S22, 0xc040b340);  /* 18 */
        c = GG(c, d, a, b, x[11], S23, 0x265e5a51); /* 19 */
        b = GG(b, c, d, a, x[0], S24, 0xe9b6c7aa);  /* 20 */
        a = GG(a, b, c, d, x[5], S21, 0xd62f105d);  /* 21 */
        d = GG(d, a, b, c, x[10], S22, 0x02441453); /* 22 */
        c = GG(c, d, a, b, x[15], S23, 0xd8a1e681); /* 23 */
        b = GG(b, c, d, a, x[4], S24, 0xe7d3fbc8);  /* 24 */
        a = GG(a, b, c, d, x[9], S21, 0x21e1cde6);  /* 25 */
        d = GG(d, a, b, c, x[14], S22, 0xc33707d6); /* 26 */
        c = GG(c, d, a, b, x[3], S23, 0xf4d50d87);  /* 27 */
        b = GG(b, c, d, a, x[8], S24, 0x455a14ed);  /* 28 */
        a = GG(a, b, c, d, x[13], S21, 0xa9e3e905); /* 29 */
        d = GG(d, a, b, c, x[2], S22, 0xfcefa3f8);  /* 30 */
        c = GG(c, d, a, b, x[7], S23, 0x676f02d9);  /* 31 */
        b = GG(b, c, d, a, x[12], S24, 0x8d2a4c8a); /* 32 */

        /* Round 3 */
        a = HH(a, b, c, d, x[5], S31, 0xfffa3942);  /* 33 */
        d = HH(d, a, b, c, x[8], S32, 0x8771f681);  /* 34 */
        c = HH(c, d, a, b, x[11], S33, 0x6d9d6122); /* 35 */
        b = HH(b, c, d, a, x[14], S34, 0xfde5380c); /* 36 */
        a = HH(a, b, c, d, x[1], S31, 0xa4beea44);  /* 37 */
        d = HH(d, a, b, c, x[4], S32, 0x4bdecfa9);  /* 38 */
        c = HH(c, d, a, b, x[7], S33, 0xf6bb4b60);  /* 39 */
        b = HH(b, c, d, a, x[10], S34, 0xbebfbc70); /* 40 */
        a = HH(a, b, c, d, x[13], S31, 0x289b7ec6); /* 41 */
        d = HH(d, a, b, c, x[0], S32, 0xeaa127fa);  /* 42 */
        c = HH(c, d, a, b, x[3], S33, 0xd4ef3085);  /* 43 */
        b = HH(b, c, d, a, x[6], S34, 0x04881d05);  /* 44 */
        a = HH(a, b, c, d, x[9], S31, 0xd9d4d039);  /* 45 */
        d = HH(d, a, b, c, x[12], S32, 0xe6db99e5); /* 46 */
        c = HH(c, d, a, b, x[15], S33, 0x1fa27cf8); /* 47 */
        b = HH(b, c, d, a, x[2], S34, 0xc4ac5665);  /* 48 */

        /* Round 4 */
        a = II(a, b, c, d, x[0], S41, 0xf4292244);  /* 49 */
        d = II(d, a, b, c, x[7], S42, 0x432aff97);  /* 50 */
        c = II(c, d, a, b, x[14], S43, 0xab9423a7); /* 51 */
        b = II(b, c, d, a, x[5], S44, 0xfc93a039);  /* 52 */
        a = II(a, b, c, d, x[12], S41, 0x655b59c3); /* 53 */
        d = II(d, a, b, c, x[3], S42, 0x8f0ccc92);  /* 54 */
        c = II(c, d, a, b, x[10], S43, 0xffeff47d); /* 55 */
        b = II(b, c, d, a, x[1], S44, 0x85845dd1);  /* 56 */
        a = II(a, b, c, d, x[8], S41, 0x6fa87e4f);  /* 57 */
        d = II(d, a, b, c, x[15], S42, 0xfe2ce6e0); /* 58 */
        c = II(c, d, a, b, x[6], S43, 0xa3014314);  /* 59 */
        b = II(b, c, d, a, x[13], S44, 0x4e0811a1); /* 60 */
        a = II(a, b, c, d, x[4], S41, 0xf7537e82);  /* 61 */
        d = II(d, a, b, c, x[11], S42, 0xbd3af235); /* 62 */
        c = II(c, d, a, b, x[2], S43, 0x2ad7d2bb);  /* 63 */
        b = II(b, c, d, a, x[9], S44, 0xeb86d391);  /* 64 */

        fState[0] += a;
        fState[1] += b;
        fState[2] += c;
        fState[3] += d;
    }
}