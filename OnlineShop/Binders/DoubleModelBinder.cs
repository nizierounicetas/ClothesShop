using System.Globalization;
using System.Web.Mvc;

namespace OnlineShop.Binders
{
    public class DoubleModelBinder : System.Web.Mvc.IModelBinder
    {
        public object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            System.Web.Mvc.ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            if (valueResult.AttemptedValue != null)
            {
                try
                {
                    actualValue = Convert.ToDouble(valueResult.AttemptedValue, CultureInfo.InvariantCulture);
                }
                catch (FormatException e)
                {
                    modelState.Errors.Add(e);
                }
            }



            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}
