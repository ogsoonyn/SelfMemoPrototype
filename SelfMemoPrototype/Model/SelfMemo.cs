using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfMemoPrototype.Model
{
    public class SelfMemo
    {
        public string Keyword { get; set; }

        public string ShortKeyword { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public DateTime Date { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SelfMemo)
            {
                bool chk1 = (obj as SelfMemo).Keyword.Equals(Keyword);
                bool chk2 = (obj as SelfMemo).Description.Equals(Description);
                bool chk3 = (obj as SelfMemo).ShortKeyword.Equals(ShortKeyword);
                bool chk4 = (obj as SelfMemo).Category.Equals(Category);

                return chk1 && chk2 && chk3 && chk4;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Keyword + Description + ShortKeyword + Category).GetHashCode();
        }

        public SelfMemo(string keyword, string shortkwd, string description, string category)
        {
            Keyword = keyword;
            ShortKeyword = shortkwd;
            Description = description;
            Category = category;
            Date = DateTime.Now;
        }
    }
}
