using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SaftbotII.Commands
{
    internal static class ConvertCommand
    {
        private static Format[] knownFormats = new Format[]
        {
            new Format()
            {
                Names = new string[]{ "hexadecimal","hex", "b16" },
                Description = "A hexadecimal number",
                Encode = (a) => {
                    string res = "0x";

                    for (int i = 0; i < a.Length; i++)
                    {
                        res += (i == 0)?a[i].ToString("X"):a[i].ToString("X").PadLeft(2, '0');
			        }

                    return res;
                },
                Decode = (a) => {
                    byte[] res = new byte[(int)Math.Ceiling(a.Length / 2.0)];
                    
                    a = a.ToUpper();
                    if(a.StartsWith("0X"))
                        a = a.Substring(2,a.Length - 2);

                    if(a.Length % 2 == 1)
                        a = '0' + a;

                    for (int i = 0; i < a.Length; i+=2)
                    {
                        string toParse = a.Substring(i, 2);
                        byte val = byte.Parse(toParse, System.Globalization.NumberStyles.HexNumber);
                        res[i/2] = val;
                    }

                    return res;
                }
            },

            new Format()
            {
                Names = new string[]{ "base64", "b64" },
                Description = "A base 64 number",
                Encode = System.Convert.ToBase64String,
                Decode = (a) => {
                    if(a.Length % 4 != 0)
                    {
                        int wantedLen = a.Length + 4 - a.Length % 4;
                        a = a.PadRight(wantedLen, '=');
                    }

                    return System.Convert.FromBase64String(a);
                }
            },

            new Format()
            {
                Names = new string[]{ "decimal", "b10" },
                Description = "A decimal number",
                Encode = (a) => { return new BigInteger(a.Reverse().ToArray()).ToString(); },
                Decode = (a) => { return BigInteger.Parse(a).ToByteArray().Reverse().ToArray(); }
            },

            new Format()
            {
                Names = new string[]{ "ascii", "text" },
                Description = "Plaintext",
                Encode  = (a) => Encoding.ASCII.GetString(a),
                Decode = (a) => Encoding.ASCII.GetBytes(a)
            },

            new Format()
            {
                Names = new string[]{ "unicode", "utf8" },
                Description = "Unicode plaintext",
                Encode  = (a) => Encoding.UTF8.GetString(a),
                Decode = (a) => Encoding.UTF8.GetBytes(a)
            }
        };

        private static string listFormats()
        {
            string msg = "Available formats are:";

            foreach (var format in knownFormats)
            {
                msg += $"\n\t{String.Join('|',format.Names)}:\t{format.Description}";
            }

            return msg;
        }

        [Command("Converts text between formats", "<-to/-from> <Format> [<-to/-from>] [<Format>] <Text>")]
        public static async void Convert(CommandInformation cmdinfo)
        {
            // Set input format to ASCII as standard
            Format? input = knownFormats[3];
            // Set input format to Unicode as standard
            Format? output = knownFormats[4];
            string mode = cmdinfo.arguments[0].ToLower();

            switch(mode)
            {
                case "-to":
                    output = findFormat(cmdinfo.arguments[1]);
                    break;

                case "-from":
                    input = findFormat(cmdinfo.arguments[1]);
                    break;

                default:
                    await cmdinfo.messages.Send("Expected '-to' or '-from' as first argument!");
                    return;
            }
            bool readExtraData = false;

            if (cmdinfo.arguments.Length >= 4)
            {
                string secondaryMode = cmdinfo.arguments[2].ToLower();

                switch (secondaryMode)
                {
                    case "-to":
                        output = findFormat(cmdinfo.arguments[3]);
                        readExtraData = true;
                    break;
                    case "-from":
                        input = findFormat(cmdinfo.arguments[3]);
                        readExtraData = true;
                        break;
            }
            }

            if(! input.HasValue)
            {
                await cmdinfo.messages.Send($"Unrecognized input format.\n{listFormats()}");
                return;
            }

            if(! output.HasValue)
            {
                await cmdinfo.messages.Send($"Unrecognized output format.\n{listFormats()}");
                return;
            }

            try
            {
                // All whitespace is treated the same; Beware
                string toConvert = String.Join(' ', Util.SubArray(cmdinfo.arguments, readExtraData ? 4: 2));
                byte[] raw = input.Value.Decode(toConvert);
                await cmdinfo.messages.Send(output.Value.Encode(raw));
            }
            catch
            {
                await cmdinfo.messages.Send("Your input was in an incorrect format!");
            }
        }

        private static Format? findFormat(string name)
        {
            name = name.ToLower();

            foreach (var f in knownFormats)
                if (f.Names.Contains(name))
                    return f;

            return null;
        }
    }

    struct Format
    {
        /// <summary>
        /// An array of names and aliases for this format
        /// </summary>
        public string[] Names;

        public string Description;

        /// <summary>
        /// A function that converts a little-endian byte array to a string representation of this format
        /// </summary>
        public Func<byte[], string> Encode;

        /// <summary>
        /// A function that converts a string of this format to a little-endian byte array
        /// </summary>
        public Func<string, byte[]> Decode;
    }
}
