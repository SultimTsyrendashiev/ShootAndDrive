using System;

namespace SD.UI
{
    class MultilineTranslatedText : TranslatedText
    {
        protected override void SetText(string localizedText)
        {
            base.SetText(localizedText.Replace("\\n", Environment.NewLine));
        }
    }
}
