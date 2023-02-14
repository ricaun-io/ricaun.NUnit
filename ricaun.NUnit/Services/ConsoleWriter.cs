using System;
using System.IO;
using System.Text;

namespace ricaun.NUnit.Services
{
    /// <summary>
    /// ConsoleWriterDateTime
    /// </summary>
    internal class ConsoleWriterDateTime : ConsoleWriter
    {
        private DateTime Time = DateTime.Now;

        /// <summary>
        /// Get Millis
        /// </summary>
        /// <returns></returns>
        public double GetMillis()
        {
            var millis = GetTimeSpan().TotalMilliseconds;
            millis = Math.Round(millis, 2);
            return millis;
        }
        private TimeSpan GetTimeSpan()
        {
            var time = Time;
            Time = DateTime.Now;
            return Time - time;
        }
    }


    /// <summary>
    /// ConsoleWriter
    /// </summary>
    internal class ConsoleWriter : IDisposable
    {
        private TextWriter consoleOut;
        private TextWriter consoleError;
        private StringWriter consoleWriter;

        /// <summary>
        /// ConsoleWriter
        /// </summary>
        /// <param name="enableConsole">Enable default Console.Out and Console.Error</param>
        public ConsoleWriter(bool enableConsole = false)
        {
            consoleOut = Console.Out;
            consoleError = Console.Error;

            consoleWriter = new StringWriter();

            Console.SetOut(consoleWriter);
            Console.SetError(consoleWriter);

            if (enableConsole)
            {
                Console.SetOut(new JoinTextWriter(consoleWriter, consoleOut));
                Console.SetError(new JoinTextWriter(consoleWriter, consoleError));
            }
        }

        /// <summary>
        /// Get String
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {
            var str = consoleWriter.ToString();
            StringBuilder sb = consoleWriter.GetStringBuilder();
            sb.Remove(0, sb.Length);
            return str;
        }

        /// <summary>
        /// Dispose and set default Console.Out and Console.Error
        /// </summary>
        public void Dispose()
        {
            Console.SetOut(consoleOut);
            Console.SetError(consoleError);
        }

        /// <summary>
        /// JoinTextWriter
        /// </summary>
        public class JoinTextWriter : TextWriter
        {
            private TextWriter[] textWriters;

            /// <summary>
            /// Join <paramref name="textWriters"/>
            /// </summary>
            /// <param name="textWriters"></param>
            public JoinTextWriter(params TextWriter[] textWriters)
            {
                this.textWriters = textWriters;
            }
            /// <summary>
            /// Write
            /// </summary>
            /// <param name="value"></param>
            public override void Write(char value)
            {
                foreach (var textWriter in textWriters)
                    textWriter?.Write(value);
                base.Write(value);
            }
            /// <summary>
            /// Encoding
            /// </summary>
            public override Encoding Encoding
            {
                get
                {
                    return textWriters[0].Encoding;
                }
            }
        }

    }

}
