using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace MD5
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter text for MD5");
                string line = Console.ReadLine();

                System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(line);
                bs = x.ComputeHash(bs); //this function is not in the above classdefinition
                System.Text.StringBuilder s = new System.Text.StringBuilder();
                foreach (byte b in bs)
                {
                    s.Append(b.ToString("x2").ToLower());
                }
                line =  "system says" + s.ToString();
                Console.WriteLine(line);
                Console.WriteLine("I say:");
                CustomMD5 customMD5 = new CustomMD5();

               byte[] m_byteInput = new byte[line.Length];
                for (int i = 0; i < line.Length; i++)
                    m_byteInput[i] = (byte)line[i];
                customMD5.Dostuff(m_byteInput);
            }
        }
    }



        public class CustomMD5
        {

            // : All variables are unsigned 32 bit and wrap modulo 2^32 when calculating
            // s specifies the per-round shift amounts
            int[] degreesOfRotation = new int[64] { 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21 };


        /// <summary>
        /// Lookup table of sin.
        /// </summary>
            readonly uint[] SinLookupTable = new uint[64]{
                0xd76aa478,0xe8c7b756,0x242070db,0xc1bdceee,
                0xf57c0faf,0x4787c62a,0xa8304613,0xfd469501,
                0x698098d8,0x8b44f7af,0xffff5bb1,0x895cd7be,
                0x6b901122,0xfd987193,0xa679438e,0x49b40821,
                0xf61e2562,0xc040b340,0x265e5a51,0xe9b6c7aa,
                0xd62f105d,0x2441453,0xd8a1e681,0xe7d3fbc8,
                0x21e1cde6,0xc33707d6,0xf4d50d87,0x455a14ed,
                0xa9e3e905,0xfcefa3f8,0x676f02d9,0x8d2a4c8a,
                0xfffa3942,0x8771f681,0x6d9d6122,0xfde5380c,
                0xa4beea44,0x4bdecfa9,0xf6bb4b60,0xbebfbc70,
                0x289b7ec6,0xeaa127fa,0xd4ef3085,0x4881d05,
                0xd9d4d039,0xe6db99e5,0x1fa27cf8,0xc4ac5665,
                0xf4292244,0x432aff97,0xab9423a7,0xfc93a039,
                0x655b59c3,0x8f0ccc92,0xffeff47d,0x85845dd1,
                0x6fa87e4f,0xfe2ce6e0,0xa3014314,0x4e0811a1,
                0xf7537e82,0xbd3af235,0x2ad7d2bb,0xeb86d391
            };


        uint A = 0x67452301;
            uint B = 0xEFCDAB89;
            uint C = 0x98BADCFE;
            uint D = 0X10325476;

            /*
     * Padding used to make the size (in bits) of the input congruent to 448 mod 512
     */
            static byte[] PADDING = new byte[] {0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

        //protected byte[] CreatePaddedBuffer(byte[] input)
        //{
        //    uint bitLength = (uint)input.Length * 8; //Number of bytes * 8 = bits
        //    uint bitLengthWithPadding = CalculateNewBitLength(input);// find the new length of the list, by addi
        //    byte[] paddedData = new byte[bitLengthWithPadding / 8];

        //    uint arrayIndex = bitLength - 1; //Index for slicing array. 
        //    uint paddingIndex = 0;
        //    do
        //    {
        //        paddedData[arrayIndex] = PADDING[paddingIndex];
        //        arrayIndex++;
        //        paddingIndex++;
        //    }
        //    while (bitLengthWithPadding - bitLength != arrayIndex + 1); //
        //                                                                //bitLengthWithPadding (448 +512 = 960) -bitLength(448) (so result is 512) !=arrayIndex+1 (448 and will increment until the 960th index!) => will stop when the two are teh same

        //    return paddedData;
        //}



        //Credit to 
        /*syed Faraz mahmood
        * Student NU FAST ICS
        * can be reached at s_faraz_mahmood@hotmail.com for this method.*/
        protected byte[] CreatePaddedBuffer(byte[] input)
        {
            uint pad;       //no of padding bits for 448 mod 512 
            byte[] bMsg;    //buffer to hold bits
            ulong sizeMsg;      //64 bit size pad
            uint sizeMsgBuff;   //buffer size in multiple of bytes
            int temp = (448 - ((input.Length * 8) % 512)); //temporary 


            pad = (uint)((temp + 512) % 512);       //getting no of bits to  be pad
            if (pad == 0)				///pad is in bits
				pad = 512;          //at least 1 or max 512 can be added

            sizeMsgBuff = (uint)((input.Length) + (pad / 8) + 8);
            sizeMsg = (ulong)input.Length * 8;
            bMsg = new byte[sizeMsgBuff];   ///no need to pad with 0 coz new bytes 
            // are already initialize to 0 :)


            ////copying string to buffer 
            for (int i = 0; i < input.Length; i++)
                bMsg[i] = input[i];

            bMsg[input.Length] |= 0x80;       ///making first bit of padding 1,

            //wrting the size value
            for (int i = 8; i > 0; i--)
                bMsg[sizeMsgBuff - i] = (byte)(sizeMsg >> ((8 - i) * 8) & 0x00000000000000ff);

            return bMsg;
        }

        private uint CalculateNewBitLength(byte[] input)
            {
                uint bitLength = (uint)input.Length * 8;
                uint bitLengthMOD512 = bitLength % 512;
                if (bitLengthMOD512 == 0)
                {
                    return bitLength + 448;
                }
                else
                {
                    return bitLength + 488 - bitLengthMOD512;
                }
            }

            private byte[] AddPaddingToCreateMultipleOf512(byte[] initialMessage)
            {
                uint initalMessageArrayLength = (uint)initialMessage.Length;
                uint bitsToIncreaseLengthBy = initalMessageArrayLength % 512;
                byte[] messageLengthInBits = BitConverter.GetBytes(initalMessageArrayLength);

                messageLengthInBits.Reverse();
                byte[] first32bits = new byte[4];
                Array.Copy(messageLengthInBits, first32bits, 4);

                byte[] last32bits = new byte[4];
                Array.Copy(messageLengthInBits, 4, last32bits, 0, 4);

                byte[] paddedMessage = new byte[initialMessage.Length + bitsToIncreaseLengthBy];
                paddedMessage = initialMessage;
                Array.Copy(first32bits, 0, paddedMessage, initalMessageArrayLength - 8 - 1, 4);
                Array.Copy(last32bits, 0, paddedMessage, initalMessageArrayLength - 4 - 1, 4);
                return paddedMessage;
            }

            private uint F(uint BB, uint CC, uint DD)
            {
                return (BB & CC) | (~BB & DD);
            }

            private uint G(uint BB, uint CC, uint DD)
            {
                return (BB & DD) | (CC & ~DD);
            }

            private uint H(uint BB, uint CC, uint DD)
            {
                return BB ^ CC ^ DD;
            }

            private uint I(uint X, uint Y, uint Z)
            {
                return Y ^ (X | ~Z);
            }



            private void Transform(uint[] array)
            {
            //

            uint AA = A;
            uint BB = B;
            uint CC = C;
            uint DD = D; 
            uint E;
            int j = 0;
            

                //Potentially looking at 8 bits.
                for (int i = 0; i <= 63; i++)
                {
                

                    if (0 <= i && i <= 15)
                    {
                        E = F(BB, CC, DD);
                        j = i;
                }
                    else if (16 <= i && i <= 31)
                    {
                        E = G(BB, CC, DD);
                        j = ((i * 5) + 1) % 16;
                }
                    else if (32 <= i && i <= 47)
                    {
                        E = H(BB, CC, DD);
                        j = ((i * 3) + 5) % 16;
                }
                    else
                    {
                        E = I(BB, CC, DD);
                        j = (i * 7) % 16;
                    //Console.WriteLine($"a: {AA} b: " +
                    //$"{BB} c:{CC} d:{DD} LUT INDEX {i}  DEGREES OF ROTATION {degreesOfRotation[i]} i : {i} j: {j} ");
                    
                }
                //Console.WriteLine($"a: {AA} b: {BB} c:{CC} d:{DD} E: {E} rotation {degreesOfRotation[i]} sin : {SinLookupTable[i]} 32 bit word: {array[j]} j{j}");
                uint temp = DD;
                //Bytes has 64 indexes
                    DD = CC;
                    CC = BB;
                    BB = BB + BitOperations.RotateLeft(AA + E + SinLookupTable[i] + array[j], degreesOfRotation[i]);
                    AA = temp;
                }
            //Console.WriteLine($"A:{A}, B:{B}, C:{C}, D:{D}");
            //Console.WriteLine($"AA:{AA}, BB:{BB}, CC:{CC}, DD:{DD}");

            //AA is incorrect. Not A. 
            A += AA;
                B += BB;
                C += CC;
                D += DD;

            //Console.WriteLine($"AA:{AA}, BB:{BB}, CC:{CC}, DD:{DD}");
            //Console.WriteLine($"A:{A}, B:{B}, C:{C}, D:{D}");
            }

            public static uint RotateLeft(uint uiNumber, ushort shift)
            {
                return ((uiNumber >> 32 - shift) | (uiNumber << shift));
            }


        public static uint ReverseByte(uint uiNumber)
        {
            return (((uiNumber & 0x000000ff) << 24) |
                        (uiNumber >> 24) |
                    ((uiNumber & 0x00ff0000) >> 8) |
                    ((uiNumber & 0x0000ff00) << 8));
        }

        public void Dostuff(byte[] input)
            {


                byte[] paddedArray = CreatePaddedBuffer(input);
            //byte[] multipleOf512Array = AddPaddingToCreateMultipleOf512(paddedArray);


            uint N = (uint)(paddedArray.Length * 8) / 32;

            for (uint i = 0; i < N / 16; i++) //In other words, just repeat once for each 64 bits.
            {
                uint[] uintBlocks =  CopyBlock(paddedArray, i);
                Transform(uintBlocks);
            }
                Console.WriteLine($"{A}{B}{C}{D}");
            Console.WriteLine(ReverseByte(A).ToString("X8") + ReverseByte(B).ToString("X8") + ReverseByte(C).ToString("X8") + ReverseByte(D).ToString("X8"));
            //Console.WriteLine($"{A+B+C+D}");
        }


        protected uint[] CopyBlock(byte[] bMsg, uint block)
        {
            uint[] X = new uint[16];

        block = block << 6;
            for (uint j = 0; j < 61; j += 4)
            {
                X[j >> 2] = (((uint)bMsg[block + (j + 3)]) << 24) |
                        (((uint)bMsg[block + (j + 2)]) << 16) |
                        (((uint)bMsg[block + (j + 1)]) << 8) |
                        (((uint)bMsg[block + (j)]));

            }

            return X;
        }
    }

    


}




