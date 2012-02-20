using System;
using System.Reflection;
using FubuCore.Conversion;

namespace FubuCore.Binding
{
    public class PropertyContext : IPropertyContext
    {
        private readonly IBindingContext _parent;
        private readonly IServiceLocator _services;
        private readonly PropertyInfo _property;

        public PropertyContext(IBindingContext parent, IServiceLocator services, PropertyInfo property)
        {
            _parent = parent;
            _services = services;
            _property = property;
        }

        string IConversionRequest.Text
        {
            get { return RawValueFromRequest.RawValue as string; }
        }

        T IConversionRequest.Get<T>()
        {
            return _parent.Service<T>();
        }

        IConversionRequest IConversionRequest.AnotherRequest(string text)
        {
            return new ConversionRequest(text, Service);
        }

        public BindingValue RawValueFromRequest
        {
            get { return _parent.Data.RawValue(Property.Name); }
        }

        public PropertyInfo Property
        {
            get { return _property; }
        }

        public object Object
        {
            get { return _parent.Object; }
        }

        public T Service<T>()
        {
            return _parent.Service<T>();
        }

        public object Service(Type typeToFind)
        {
            return _services.GetInstance(typeToFind);
        }

        public void WithValue(Action<object> continuation)
        {
            RequestData.Value(Property.Name, value =>
            {
                // TODO -- log the BindingValue here
                continuation(value.RawValue);
            });
        }

        T IPropertyContext.ValueAs<T>()
        {
            return _parent.Data.ValueAs<T>(_property.Name);
        }

        bool IPropertyContext.ValueAs<T>(Action<T> continuation)
        {
            return _parent.Data.ValueAs(_property.Name, continuation);
        }

        public IBindingLogger Logger
        {
            get { return _parent.Logger; }
        }

        public IRequestData RequestData
        {
            get { return _parent.RequestData; }
        }

        public void SetPropertyValue(object value)
        {
            Property.SetValue(Object, value, null);
        }

        public object GetPropertyValue()
        {
            return Property.GetValue(Object, null);
        }
    }
}