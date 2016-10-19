using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace System.Web.Mvc
{
    public class Html5Helper
    {
        private readonly HtmlHelper htmlHelper;

        public Html5Helper(HtmlHelper htmlHelper)
        {
            this.htmlHelper = htmlHelper;
        }
        private static CultureInfo Culture
        {
            get
            {
                return CultureInfo.CurrentCulture;
            }
        }

        // Add custom methods here...
        public IHtmlString EmailControl()
        {
            string id;
            string name;
            string placeHolder;
            string value;
            string valueAttribute;

            ViewDataDictionary viewData = htmlHelper.ViewData;
            ModelMetadata metaData = viewData.ModelMetadata;

            // Build the HTML attributes
            id = viewData.TemplateInfo.GetFullHtmlFieldId(string.Empty);
            name = viewData.TemplateInfo.GetFullHtmlFieldName(string.Empty);

            if (string.IsNullOrWhiteSpace(metaData.Watermark))
                placeHolder = string.Empty;
            else
                placeHolder = "placeholder=\"" + metaData.Watermark + "\"";

            value = viewData.TemplateInfo.FormattedModelValue.ToString();
            if (string.IsNullOrWhiteSpace(value))
                valueAttribute = string.Empty;
            else
                valueAttribute = "value=\"" + value + "\"";

            // Determine the css class
            string css = "text-box single-line";

            ModelState state;
            if (viewData.ModelState.TryGetValue(name, out state)
                && (state.Errors.Count > 0))
                css += " " + HtmlHelper.ValidationInputCssClassName;

            // Format the final HTML
            string markup = string.Format(Culture,
                "<input type=\"email\" id=\"{0}\" name=\"{1}\" {2} {3} " +
                "class=\"{4}\"/>", id, name, placeHolder, valueAttribute, css);

            return MvcHtmlString.Create(markup);
        }

        public IHtmlString RangeControl(int min, int max, int step)
        {
            string id;
            string name;
            string value;
            string valueAttribute;

            ViewDataDictionary viewData = htmlHelper.ViewData;

            // Build the HTML attributes
            id = viewData.TemplateInfo.GetFullHtmlFieldId(string.Empty);
            name = viewData.TemplateInfo.GetFullHtmlFieldName(string.Empty);

            value = viewData.TemplateInfo.FormattedModelValue.ToString();
            if (string.IsNullOrWhiteSpace(value))
                valueAttribute = string.Empty;
            else
                valueAttribute = "value=\"" + value + "\"";

            // Determine the css class
            string css = "range";

            ModelState state;
            if (viewData.ModelState.TryGetValue(name, out state)
                && (state.Errors.Count > 0))
                css += " " + HtmlHelper.ValidationInputCssClassName;

            // Format the final HTML
            string markup = string.Format(Culture,
                "<input type=\"range\" id=\"{0}\" name=\"{1}\" " +
                "min=\"{2}\" max=\"{3}\" step=\"{4}\" {5} class=\"{6}\"/>",
                id, name, min.ToString(), max.ToString(), step.ToString(),
                valueAttribute, css);

            return MvcHtmlString.Create(markup);
        }
    }

    public static class HtmlHelperExtension
    {
        public static Html5Helper Html5(this HtmlHelper instance)
        {
            return new Html5Helper(instance);
        }
    }
}
