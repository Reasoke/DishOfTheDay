using System.Text;

namespace DishOfTheDay
{
    internal class RtfBuilder
    {
        StringBuilder builder = new StringBuilder();

        public RtfBuilder AppendBold(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                builder.Append(@"\b ");
                builder.Append(text);
                builder.Append(@"\b0 ");
            }
            return this;
        }

        public RtfBuilder AppendBoldLine(string text)
        {
            return AppendBold(text).AppendLine();
        }

        public RtfBuilder Append(string text)
        {
            if (!string.IsNullOrEmpty(text))
                builder.Append(text);
            return this;
        }

        public RtfBuilder AppendLine(string text = null)
        {
            if (!string.IsNullOrEmpty(text))
                builder.Append(text);
            builder.AppendLine(@"\line");
            return this;
        }

        public string ToRtf()
        {
            return $@"{{\rtf1\ansi {builder.ToString()} }}";
        }
    }

}
