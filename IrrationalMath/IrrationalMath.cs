public class IrrationalMath
{
    public struct BigFloat
    {
        public readonly long[] Bits;
        public readonly int Precision;
        public byte Type; //0: number, 1: NaN, 2: (negative) Infinity
        public bool Sign;

        public BigFloat(long[] bits, int precision, bool sign)
        {
            Bits = new long[bits.Length];
            Array.Copy(bits, Bits, bits.Length);
            Precision = precision;
            Sign = sign;
            Type = 0;
        }

        public BigFloat(BigFloat bigFloat)
        {
            Bits = new long[bigFloat.Bits.Length];
            Array.Copy(bigFloat.Bits, Bits, bigFloat.Bits.Length);
            Precision = bigFloat.Precision;
            Sign = bigFloat.Sign;
            Type = bigFloat.Type;
        }

        public BigFloat(long number)
        {
            Bits = new long[1] { number };
            Precision = 0;
            Sign = (number >> 63 & 1) == 1;
            Type = 0;
        }

        public BigFloat(ulong number)
        {
            if ((number >> 63 & 1) == 1)
                Bits = new long[2] { (long)(number & 0x7FFFFFFFFFFFFFFF), 1 };
            else
                Bits = new long[1] { (long)number };
            Precision = 0;
            Sign = false;
            Type = 0;
        }

        public BigFloat(int number)
        {
            if (number < 0)
            {
                Bits = [number & 0x7FFFFFFFFFFFFFFF];
                Sign = true;
            }
            else
            {
                Bits = [number];
                Sign = false;
            }

            Precision = 0;
            Type = 0;
        }

        public BigFloat(uint number)
        {
            Bits = [number];
            Precision = 0;
            Sign = false;
            Type = 0;
        }

        public BigFloat(byte number)
        {
            Bits = [number];
            Precision = 0;
            Sign = false;
            Type = 0;
        }

        public override readonly string ToString()
        {
            BigFloat number;
            bool sign = Sign;
            if (sign == true)
                number = -this;
            else
                number = this;

            byte[] afterDecimalPoint = new byte[Precision];
            List<byte> beforeDecimalPoint = new() { 0 };

            for (int i = 0; i < Precision; i++)
            {
                for (int j = 0; j < afterDecimalPoint.Length; j++)
                {
                    if ((afterDecimalPoint[^(j + 1)] & 1) == 1)
                        afterDecimalPoint[^j] += 5;
                    afterDecimalPoint[^(j + 1)] >>= 1;
                }

                if ((number.Bits[0] & 1) == 1)
                    afterDecimalPoint[0] += 5;
                number >>= 1;
            }

            for (int i = 1; i <= Bits.Length; i++)
            {
                for (int n = 0; n < 63; n++)
                {
                    number.Bits[^i] <<= 1;
                    for (int j = 0; j < beforeDecimalPoint.Count; j++)
                    {
                        beforeDecimalPoint[j] = CorrectByte(beforeDecimalPoint[j]); //If the byte is > 4 it will add 3    100 => 100, 101 => 1000, 110 => 1001, etc
                        beforeDecimalPoint[j] <<= 1; //Moves all bits one to the left to make space for new bits in the next round
                    }
                    CorrectBytes(beforeDecimalPoint);
                    beforeDecimalPoint[0] += (byte)((number.Bits[^i] >> 63) * -1); //Adds the next number en the sequnce the the smallest number in the output
                }
            }

            return (sign ? "-" : "") + Write(beforeDecimalPoint) + "." + Write(afterDecimalPoint);
        }

        public readonly string ToString(string format)
        {
            string outString = "";
            switch (format)
            {
                case "raw":
                    outString = Sign ? "- " : "+ ";
                    for (int i = 1; i <= Bits.Length; i++)
                    {
                        outString += Bits[^i].ToString();
                        if (i < Bits.Length)
                            outString += ", ";
                    }
                    outString += "; " + Precision;
                    return outString;

                case "dec":
                    BigFloat number;

                    if (Sign == true)
                        number = -this;
                    else
                        number = new BigFloat(Bits.ToArray(), Precision, Sign);

                    List<byte> numbers = [0];

                    for (int i = 1; i <= Bits.Length; i++)
                    {
                        for (int n = 0; n < 63; n++)
                        {
                            number.Bits[^i] <<= 1;
                            for (int j = 0; j < numbers.Count; j++) //Calculates for each byte
                            {
                                numbers[j] = CorrectByte(numbers[j]); //If the byte is > 4 it will add 3    100 => 100, 101 => 1000, 110 => 1001, etc
                                numbers[j] <<= 1; //Moves all bits one to the left to make space for new bits in the next round
                            }
                            CorrectBytes(numbers);
                            numbers[0] += (byte)((number.Bits[^i] >> 63) * -1); //Adds the next number en the sequnce the the smallest number in the output
                        }
                    }
                    outString = (Sign ? "-" : "") + Write(numbers) + ";" + Precision;
                    return outString;

                case "B":
                    for (int i = 1; i < Bits.Length; i++)
                    {
                        outString = " " + LongToBinary(Bits[i - 1]) + outString;
                    }
                    outString = LongToBinary(Bits[^1], false) + outString + " " + Convert.ToString(Precision, 2).PadLeft(32, '0');
                    return (Sign ? "1 " : "0 ") + outString;

                case "b":
                    for (int i = 1; i < Bits.Length; i++)
                    {
                        outString = " " + LongToBinary(Bits[i - 1]) + outString;
                    }
                    outString = LongToBinary(Bits[^1], false) + outString + " " + Convert.ToString(Precision, 2).PadLeft(32, '0');
                    return (Sign ? "1 " : "0 ") + outString;

                case "X":
                    foreach (long bite in Bits)
                        outString = bite.ToString("X") + outString;
                    while (outString.Length < 8)
                        outString = '0' + outString;
                    return outString;

                case "x":
                    foreach (long bite in Bits)
                        outString = bite.ToString("X") + outString;
                    while (outString.Length < 8)
                        outString = '0' + outString;
                    return outString;

                default:
                    //throw new ArgumentException("UNKOWN FORMAT");
                    //Debug.LogError("UNKOWN FORMAT");
                    return ToString();
            }
        }

        static (long, bool) PartialAddition(long a, long b, bool rest)
        {
            long c = a + b;
            if (rest)
                c += 1;
            bool sign = c < 0;
            if (sign)
                c &= 0x7FFFFFFFFFFFFFFFL;
            return (c, sign);
        }

        public static BigFloat operator +(BigFloat a, BigFloat b)
        {
            (long, bool) PartialResult;
            BigFloat output = new(new long[MostBits(a, b)], 0, false);
            bool rest = false;

            int leastBits = Math.Min(a.Bits.Length, b.Bits.Length);
            int n;
            for (n = 0; n < leastBits; n++) // Adds indivdual Bits until either a or b has run out of bits
            {
                PartialResult = PartialAddition(a.Bits[n], b.Bits[n], rest);
                output.Bits[n] = PartialResult.Item1;
                rest = PartialResult.Item2;
            }

            n--;

            if (a.Bits.Length > leastBits) // If "a" still has Bits, it will add "a" and rest and 0 if the sign is false and 2^63 if it's true for each bit
            {
                output.Sign = a.Sign;
                long additive = b.Sign ? 0x7FFFFFFFFFFFFFFF : 0;

                while (n + 1 < a.Bits.Length)
                {
                    n++;

                    PartialResult = PartialAddition(a.Bits[n], additive, rest);
                    output.Bits[n] = PartialResult.Item1;
                    rest = PartialResult.Item2;
                }
            }
            else if (b.Bits.Length > leastBits) // If "b" still has Bits, it will add "b" and rest and 0 if the sign is false and 2^63 if it's true for each bit
            {
                output.Sign = b.Sign;
                long additive = a.Sign ? 0x7FFFFFFFFFFFFFFF : 0;

                while (n + 1 < b.Bits.Length)
                {
                    n++;

                    PartialResult = PartialAddition(b.Bits[n], additive, rest);
                    output.Bits[n] = PartialResult.Item1;
                    rest = PartialResult.Item2;
                }
            }
            else
            {
                output.Sign = (a.Sign ^ b.Sign) ^ rest;
                return output;
            }

            // Handles the case where the end result has more Bits than either a or b
            if (rest && (a.Sign == b.Sign))
            {
                long[] newBits = new long[output.Bits.Length + 1];
                Array.Copy(output.Bits, newBits, output.Bits.Length);
                newBits[n + 1] = a.Sign ? 0x7FFFFFFFFFFFFFFE : 1;
                return new(newBits, output.Precision, a.Sign);
            }
            return output;
        }

        public static BigFloat operator -(BigFloat a, BigFloat b)
        {
            return a + -b;
        }

        public static BigFloat operator -(BigFloat a)
        {
            BigFloat output = new(new long[a.Bits.Length], a.Precision, !a.Sign);
            bool rest = true;
            for (int i = 0; i < a.Bits.Length; i++)
            {
                if (rest)
                {
                    output.Bits[i] = ((~a.Bits[i]) & 0x7fffffffffffffff) + 1;
                    rest = output.Bits[i] < 0;
                    if (rest)
                        output.Bits[i] &= 0x7fffffffffffffff;
                }
                else
                {
                    output.Bits[i] = (~a.Bits[i]) & 0x7fffffffffffffff;
                }
            }
            return output;
        }

        public static BigFloat operator <<(BigFloat a, uint b)
        {
            uint extraBytes = b / 63;
            long[] outputBits = new long[a.Bits.Length + extraBytes];
            long extraBit = 0;
            b %= 63;
            bool rest = false;

            if (b > 0)
            {
                for (int i = 0; i < a.Bits.Length; i++)
                {
                    outputBits[i + extraBytes] = a.Bits[i] << 1;
                    if (rest)
                        outputBits[i + extraBytes] += 1;
                    rest = outputBits[i + extraBytes] < 0;
                    if (rest)
                        outputBits[i + extraBytes] &= 0x7FFFFFFFFFFFFFFF;
                }

                for (int j = 1; j < b; j++)
                {
                    if (rest)
                        extraBit += 1;
                    if (extraBit != 0)
                        extraBit <<= 1;
                    rest = false;
                    for (int i = 0; i < a.Bits.Length; i++)
                    {
                        outputBits[i + extraBytes] <<= 1;
                        if (rest)
                            outputBits[i + extraBytes] += 1;
                        rest = outputBits[i + extraBytes] < 0;
                        if (rest)
                            outputBits[i + extraBytes] &= 0x7FFFFFFFFFFFFFFF;
                    }
                }
                if (rest)
                    extraBit += 1;
                if (extraBit != 0)
                {
                    long[] newArray = new long[outputBits.Length + 1];
                    Array.Copy(outputBits, newArray, outputBits.Length);
                    newArray[^1] = extraBit;

                    return new(newArray, a.Precision, a.Sign);
                }
            }
            else
            {
                long[] extendedBits = new long[a.Bits.Length + extraBytes];
                Array.Copy(a.Bits, 0, extendedBits, extraBytes, a.Bits.Length);
                return new BigFloat(extendedBits, a.Precision, a.Sign);
            }

            return new(outputBits, a.Precision, a.Sign);
        }

        public static BigFloat operator >>(BigFloat a, uint b)
        {
            uint removedBytes = b / 63;
            if (removedBytes >= a.Bits.Length)
                return new BigFloat([], 0, a.Sign);
            b %= 63;
            long[] outputBits = new long[a.Bits.Length - removedBytes];

            if (b != 0)
            {
                outputBits[0] = a.Bits[0 + removedBytes] >> 1;
                for (int i = 1; i < a.Bits.Length - removedBytes; i++)
                {
                    if ((a.Bits[i + removedBytes] & 1) == 1)
                        outputBits[i - 1] += 0x4000000000000000;
                    outputBits[i] = a.Bits[i + removedBytes] >> 1;
                }

                for (int j = 1; j < b; j++)
                {
                    outputBits[0] >>= 1;
                    for (int i = 1; i < outputBits.Length; i++)
                    {
                        if ((outputBits[i] & 1) == 1)
                            outputBits[i - 1] += 0x4000000000000000;
                        outputBits[i] >>= 1;
                    }
                }
            }
            else
            {
                int newLength = a.Bits.Length - (int)removedBytes;
                long[] reducedBits = new long[newLength];
                Array.Copy(a.Bits, removedBytes, reducedBits, 0, newLength);
                return new BigFloat(reducedBits, a.Precision, a.Sign);
            }

            return new(outputBits, a.Precision, a.Sign);
        }

        public static BigFloat operator ~(BigFloat a)
        {
            BigFloat output = new(new long[a.Bits.Length], ~a.Precision, !a.Sign);
            for (int i = 0; i < a.Bits.Length; i++)
                output.Bits[i] = (~a.Bits[i]) & 0x7fffffffffffffff;
            return output;
        }
    }

    static int MostBits(BigFloat a, BigFloat b)
    {
        if (a.Bits.Length > b.Bits.Length)
            return a.Bits.Length;
        else
            return b.Bits.Length;
    }

    public static string LongToBinary(long l)
    {
        string outString = "";

        for (int i = 1; i < 63; i++)
        {
            char bit = (char)((l & 1) + '0');
            outString = bit + outString;
            l >>= 1;
        }

        return outString;
    }

    public static string LongToBinary(long l, bool keepEndZeros)
    {
        if (keepEndZeros == true)
            return LongToBinary(l);

        string outString = "";

        for (int i = 1; i < 63; i++)
        {
            char bit = (char)((l & 1) + '0');
            outString = bit + outString;
            l >>= 1;
            if (l == 0)
                return outString;
        }

        return outString;
    }

    static byte CorrectByte(byte b)
    {
        if (b > 4) return (byte)(b + 3);
        return b;
    }

    static void CorrectBytes(List<byte> numbers) //USES LIST !!!
    {
        for (int i = 0; i < numbers.Count; i++)
        {
            if (numbers[i] > 0b1111)
            {
                if (numbers.Count - 1 == i)
                    numbers.Add(1);
                else
                    numbers[i + 1]++;
                numbers[i] &= 0b1111;
            }
        }
    }

    static string Write(List<byte> bytes) //USES LIST !!!
    {
        string output = "";
        foreach (byte b in bytes)
            output = b.ToString() + output;
        return output;
    }

    static string Write(byte[] bytes)
    {
        return bytes.Aggregate("", (s, b) => s + (char)(b + '0'));
    }
}