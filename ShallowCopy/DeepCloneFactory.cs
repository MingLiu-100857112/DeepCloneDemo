using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace DeepCloneFactory
{
    public static class DeepClone_Reflection
    {
        public static object DeepClone(object model)
        {
            return DeepClone(model, string.Empty);
        }

        public static object DeepClone(object sourceObj, string propertyAllow)
        {
            Dictionary<object, object> source_cloned = new Dictionary<object, object>();
            return DeepClone(sourceObj, propertyAllow, source_cloned);
        }

        /// <summary>
        /// decide which type or interface to use, IList is idealy for object (array), not value type 
        /// </summary>
        /// <param name="sourceObj"></param>
        /// <param name="propertyAllowType"></param>
        /// <param name="source_cloned"></param>
        /// <returns></returns>
        private static object DeepClone(object sourceObj, string propertyAllowType, Dictionary<object, object> source_cloned)
        {
            ///Filter begin
            if (sourceObj == null)
                return null;

            if (sourceObj.GetType().IsValueType)
                return sourceObj;

            if (sourceObj is string)
                return sourceObj;

            if (source_cloned.ContainsKey(sourceObj))
                return source_cloned[sourceObj];

            //create a shell like object
            object cloneShell = null;
            cloneShell = Activator.CreateInstance(sourceObj.GetType());
            //creat a key pair
            source_cloned[sourceObj] = cloneShell;

            if (cloneShell == null)
                return null;

            //https://msdn.microsoft.com/zh-cn/library/kyaxdd3x%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396

            foreach (PropertyInfo propElement in sourceObj.GetType().GetProperties())
            {
                if (!propElement.CanRead)
                    continue;

                if (PropertyElementCloneAllowed(sourceObj, propertyAllowType, propElement))
                    continue;

                //if the property type is IList usually for detect the object type not value type
                if (propElement.PropertyType.GetInterface("IList") != null)
                {
                    //get the property of IList
                    IList sourceValueCollection = propElement.GetValue(sourceObj, null) as IList;

                    //if source's value is null, then set the clone to null value too 
                    if (sourceValueCollection == null)
                    {
                        //https://msdn.microsoft.com/en-us/library/xb5dd1f1(v=vs.110).aspx
                        //https://msdn.microsoft.com/zh-cn/library/b05d59ty(v=vs.110).aspx
                        if (propElement.CanWrite)
                            propElement.SetValue(cloneShell, null, null);
                        continue;
                    }

                    //if source's value is not null
                    //create a value collection "cloneValueCollection" as IList
                    //note that it is in the foreach loop, and cloneValueCollection value will change
                    IList cloneValueCollection = propElement.GetValue(cloneShell, null) as IList;

                    //get the model property's value's type, this is the true TYPE, not interface
                    Type sourceValueType = sourceValueCollection.GetType();

                    //if model property's value's type is array
                    if (sourceValueType.IsArray)
                    {
                        if (propElement.CanWrite)
                        {
                            //shallow copy the source to cloned
                            cloneValueCollection = (sourceValueCollection as Array).Clone() as IList;

                            //final step for array clone!
                            propElement.SetValue(cloneShell, cloneValueCollection, null);
                        }
                    }
                    //if model property's value's type is not array, exp. List<>
                    else
                    {
                        //Every collection type, including arrays and IEnumerable<T>, implements IEnumerable
                        //if cloneCollection is null
                        if (cloneValueCollection == null)
                        {
                            cloneValueCollection = Activator.CreateInstance(sourceValueType) as IList;
                            propElement.SetValue(cloneShell, cloneValueCollection, null);
                        }
                        //if cloneCollection not null, you have to use add, not Clone() method because this is not array
                        //Below is not optional, for List<> copy
                        foreach (object elem in sourceValueCollection)
                        {
                            cloneValueCollection.Add(DeepClone(elem, propertyAllowType, source_cloned));
                        }
                    }
                    continue;
                }

                //if the property type is NOT IList
                if (!propElement.CanWrite)
                    continue;

                //if property is value type, most simple handle process.
                if (propElement.PropertyType.IsValueType)
                {
                    propElement.SetValue(cloneShell, propElement.GetValue(sourceObj, null), null);
                }
                //if property is not value type, recur would happen, until it get the value
                else
                {
                    propElement.SetValue(cloneShell, DeepClone(propElement.GetValue(sourceObj, null), propertyAllowType, source_cloned), null);
                }
            }

            return cloneShell;
        }

        /// <summary>
        /// This method is used to set bool switch allowing the origin's propery can be deep cloned or not
        /// </summary>
        /// <param name="sourceObj">origin object</param>
        /// <param name="propertyAllowType">property allowed type</param>
        /// <param name="propertyCollection">property element in property collection</param>
        /// <returns></returns>
        private static bool PropertyElementCloneAllowed(object sourceObj, string propertyAllowType, PropertyInfo propertyCollection)
        {
            //set the bool value by default
            bool notAllow = false;
            //get the property elements array
            object[] customAttributeList = propertyCollection.GetCustomAttributes(typeof(CloneAllowAttribute), false);
            //if property exsit
            if (customAttributeList != null && customAttributeList.Length > 0)
            {
                //loop for each property for screening
                foreach (CloneAllowAttribute customAttribute in customAttributeList)
                {
                    //if there are no set string lable for allowing
                    if (string.IsNullOrEmpty(customAttribute.AllowPropertyType))
                    {
                        //if the dicision enum is set not allow
                        if (customAttribute.Decision == CloneSwitch.NOT_ALLOW)
                            notAllow = true;
                    }
                    //if there are set string lable for allowing
                    else
                    {
                        //if the set string lable for allowing is the property type
                        if (customAttribute.AllowPropertyType == propertyAllowType)
                        {
                            //if the dicision enum is set not allow
                            if (customAttribute.Decision == CloneSwitch.ALLOW)
                            {
                                notAllow = false;
                                break;
                            }
                        }
                    }
                }
            }
            return notAllow;
        }
    }

    /// <summary>
    /// Set the use of property attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CloneAllowAttribute : Attribute
    {
        private CloneSwitch _switch;
        private string _propertyType;

        //constructor accept no aug
        public CloneAllowAttribute() : this(CloneSwitch.ALLOW) { }

        //constructor accept enum aug
        public CloneAllowAttribute(CloneSwitch cs) : this(cs, string.Empty) { }

        //constructor accept enum and string aug
        public CloneAllowAttribute(CloneSwitch cs, string pt)
        {
            _switch = cs;
            _propertyType = pt;
        }

        //property to get the enum value
        public CloneSwitch Decision
        {
            get { return _switch; }
        }

        //property to get the string value
        public string AllowPropertyType
        {
            get { return _propertyType; }
        }
    }

    //set two status of switch
    public enum CloneSwitch
    {
        ALLOW,
        NOT_ALLOW
    }
}
