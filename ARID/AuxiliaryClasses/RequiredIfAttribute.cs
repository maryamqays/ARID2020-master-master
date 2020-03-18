using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class RequiredIfAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' is required";
        private readonly object _typeId = new object();

        private string _requiredProperty;
        private string _targetProperty;
        private bool _targetPropertyCondition;

        public RequiredIfAttribute(string requiredProperty, string targetProperty, bool targetPropertyCondition)
            : base(_defaultErrorMessage)
        {
            this._requiredProperty = requiredProperty;
            this._targetProperty = targetProperty;
            this._targetPropertyCondition = targetPropertyCondition;
        }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString, _requiredProperty, _targetProperty, _targetPropertyCondition);
        }

        public override bool IsValid(object value)
        {
            bool result = false;
            bool propertyRequired = false; // Flag to check if the required property is required.

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            string requiredPropertyValue = (string)properties.Find(_requiredProperty, true).GetValue(value);
            bool targetPropertyValue = (bool)properties.Find(_targetProperty, true).GetValue(value);

            if (targetPropertyValue == _targetPropertyCondition)
            {
                propertyRequired = true;
            }

            if (propertyRequired)
            {
                //check the required property value is not null
                if (requiredPropertyValue != null)
                {
                    result = true;
                }
            }
            else
            {
                //property is not required
                result = true;
            }

            return result;
        }
    }
}
