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
                names = new string[]{ "hexadecimal", "hexadec" ,"hex", "b16", "base16" },
                Encode = (a) => {
                    //TODO: Fix the bug where every number is followed by a superfluous zero
                    string res = "";

                    foreach (byte b in a)
                        res += b.ToString("X");

                    return res;
                },
                Decode = (a) => {
                    byte[] res = new byte[a.Length / 2];
                    a = a.ToUpper();

                    for (int i = 0; i < a.Length; i++)
                        res[i/2] = byte.Parse(a.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);

                    return res;
                }
            },

            new Format()
            {
                names = new string[]{ "base64", "b64" },
                Encode = System.Convert.ToBase64String,
                Decode = System.Convert.FromBase64String
            },

            new Format()
            {
                names = new string[]{ "decimal", "base10", "b10" },
                Encode = (a) => { return new BigInteger(a).ToString(); },
                Decode = (a) => { return BigInteger.Parse(a).ToByteArray(); }
            },

            new Format()
            {
                names = new string[]{ "unicode", "plaintext", "text" },
                Encode  = (a) => ASCIIEncoding.Unicode.GetString(a),
                Decode = (a) => ASCIIEncoding.Unicode.GetBytes(a)
            }
        };

        [Command("Converts text between formats", "<-to/-from> <Format> [<-to/-from>] [<Format>] <Text>")]
        public static async void Convert(CommandInformation cmdinfo)
        {
            Format? input = knownFormats[3];
            Format? output = knownFormats[3];

            if (cmdinfo.arguments[0] == "-to")
                output = findFormat(cmdinfo.arguments[1]);
            else if (cmdinfo.arguments[0] == "-from")
                input = findFormat(cmdinfo.arguments[1]);
            else
            {
                await cmdinfo.messages.Send("Expected 'to' or 'from' as first argument!");
                return;
            }

            bool readExtraData = false;

            if(cmdinfo.arguments.Length >= 4)
            {
                if (cmdinfo.arguments[2] == "-to")
                {
                    output = findFormat(cmdinfo.arguments[3]);
                    readExtraData = true;
                }
                else if (cmdinfo.arguments[2] == "-from")
                {
                    input = findFormat(cmdinfo.arguments[3]);
                    readExtraData = true;
                }
            }

            if(! input.HasValue)
            {
                await cmdinfo.messages.Send("Unrecognized input format.");
                return;
            }

            if(! output.HasValue)
            {
                await cmdinfo.messages.Send("Unrecognized output format.");
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
                if (f.names.Contains(name))
                    return f;

            return null;
        }
    }

    struct Format
    {
        public string[] names;
        public Func<byte[], string> Encode;
        public Func<string, byte[]> Decode;
    }
}
