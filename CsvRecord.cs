using System;
using System.Collections.Generic;
using System.Text;

namespace kaiten
{
    class CsvRecord
    {
        public String Title { get; set; }
        public String Desccription { get; set; }

        protected CsvRecord(String title, String description)
        {
            Title = title;
            Desccription = description;
        }

        public static CsvRecord FromKaiten(KaitenCardRecord card)
        {
            return new CsvRecord(card.Title,
                String.Format("{0} {1}", card.Link, card.Description));
        }

    }
}
