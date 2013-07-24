using System.Web.Mvc;

namespace eCentral.Web.Framework.Mvc
{
    public class SiteModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if (model is BaseModel) ((BaseModel) model).BindModel(controllerContext, bindingContext);
            return model;
        }
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            var value = base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
            return value;
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
        protected override System.ComponentModel.PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            bindingContext.PropertyFilter = new System.Predicate<string>(pred);
            var values                    = base.GetModelProperties(controllerContext, bindingContext);

            return values;
        }

        protected bool pred(string target)
        {
            return true;
        }
    }
}
