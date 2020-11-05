using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace StringCalculator
{
    public static class StringCalculator
    {
        /// <summary>
        /// A simple string calculator which adds together specified numbers separated by commas. A custom
        /// delimiter can be specified in the input before the number in the following way: //[delimiter]\n[numbers…]
        /// </summary>
        /// <param name="numbers">String input containing numbers and optional delimiter header.</param>
        /// <returns>Sum of numbers.</returns>
        public static int Add(string numbers)
        {
            // Default delimiter.
            char[] delims = new[] { ',' };

            // Default cases.
            if (numbers == string.Empty)
            {
                return 0;
            }

            if (numbers == null)
            {
                throw new ArgumentNullException(nameof(numbers));
            }

            // Process header.
            if (numbers.SafeSubstring(0, 2) == "//")
            {
                // We have the start of the delimiter header of our input, worth trying to parse at this stage.
                // The assumption is made that a newline cannot be used as a delimiter.
                var constraints = new DelimiterHeaderConstraints()
                {
                    StartsWith = "//",
                    EndsWith = "\n"
                };

                var delimsFromHeader = TryGetDelimHeader(numbers, constraints);

                if (delimsFromHeader != null)
                {
                    if (delimsFromHeader.Contains('/'))
                    {
                        throw new ArgumentOutOfRangeException(nameof(numbers), "Invalid delimiter header specified: '/'");
                    }

                    // We have found delims, so lets cut off the header up to the first newline encountered located AFTER the user specified delims.
                    // This allows the input have any number of newlines after the delim header.
                    var indexOfLastDelim = numbers.IndexOf(delimsFromHeader.Last());
                    numbers = numbers.Substring(indexOfLastDelim + 1);
                    numbers = numbers.Substring(numbers.IndexOf('\n') + 1);

                    // Brief point 4a states the requirements to "change" the delimiter, so we overwrite the default delim instead of adding to it.
                    delims = delimsFromHeader;
                }
            }

            // Process numbers.
            var total = 0;
            var negativeAudit = "";

            foreach (var strNum in numbers.Split(delims))
            {
                bool hasNegative = false;

                int? parseResult = int.TryParse(strNum, out var num) ? num : default(int?);

                if (parseResult == null)
                {
                    throw new ArgumentNullException(nameof(numbers), "Invalid characters specified.");
                }

                if (num >= 1000)
                {
                    continue;
                }

                if (num < 0)
                {
                    hasNegative = true;
                    negativeAudit += num + ",";
                }

                if (!hasNegative)
                {
                    total += num;
                }
            }

            if (!string.IsNullOrEmpty(negativeAudit))
            {
                throw new ArgumentOutOfRangeException(nameof(numbers), $"Negatives not allowed: {negativeAudit.TrimEnd(',')}");
            }

            return total;
        }

        // Avoid index out of bounds errors with this extension.
        public static string SafeSubstring(this string orig, int start, int length)
        {
            return orig.Substring(start, orig.Length >= length ? length : orig.Length);
        }

        private static char[] TryGetDelimHeader(string numbers, DelimiterHeaderConstraints constraints)
        {
            string delimHeaderMatchPattern = @$"(?<={constraints.StartsWith}).*(?={constraints.EndsWith})";

            var match = Regex.Match(numbers, delimHeaderMatchPattern);

            if (match.Success)
            {
                return match.Value.ToCharArray();
            }

            return null;
        }

        // Constraints for the delimiter header.
        internal class DelimiterHeaderConstraints
        {
            public string StartsWith { get; set; }
            public string EndsWith { get; set; }
        }
    }
}