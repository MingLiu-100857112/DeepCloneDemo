using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace AttributeDeepClone
{
    #region Documentation Tags
    /// <summary>
    ///     A utility class which performs deep cloning of objects. Also allows to define custom 
    ///     scenarios to exclude some particular sub-objects from cloning. It is achieved by
    ///     defining <see cref="Rubenhak.Utils.CloneableAttribute"/> on target properties.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Class Information:
    ///	    <list type="bullet">
    ///         <item name="authors">Authors: Ruben Hakopian</item>
    ///         <item name="date">March 2009</item>
    ///         <item name="originalURL">http://www.rubenhak.com/?p=70</item>
    ///     </list>
    /// </para>
    /// </remarks>
    #endregion
    public static class ObjectCloner
    {
        #region Public Methods

        public static object Clone(object model)
        {
            return Clone(model, string.Empty);
        }

        public static object Clone(object model, string flavor)
        {
            Dictionary<object, object> cloneDictionary = new Dictionary<object, object>();
            return Clone(model, flavor, cloneDictionary);
        }

        #endregion

        #region Private Methods

        private static object Clone(object model, string flavor, Dictionary<object, object> cloneDictionary)
        {
            #region Filter the special situation
            ///Filter begin
            if (model == null)
                return null;

            if (model.GetType().IsValueType)
                return model;

            if (model is string)
                return model;

            if (cloneDictionary.ContainsKey(model))
                return cloneDictionary[model];
            ///Filter end
            #endregion

            #region Create object
            object clone = null;

            try
            {
                clone = Activator.CreateInstance(model.GetType());
            }
            catch
            {
            }
            #endregion

            cloneDictionary[model] = clone;

            if (clone == null)
                return null;

            foreach (PropertyInfo prop in model.GetType().GetProperties())
            {
                if (!prop.CanRead)
                    continue;

                if (ExcludeProperty(model, flavor, prop))
                    continue;

                if (prop.PropertyType.GetInterface("IList") != null)
                {
                    IList origCollection = prop.GetValue(model, null) as IList;
                    if (origCollection == null)
                    {
                        if (prop.CanWrite)
                            prop.SetValue(clone, null, null);

                        continue;
                    }

                    IList cloneCollection = prop.GetValue(clone, null) as IList;

                    Type t = origCollection.GetType();

                    if (t.IsArray)
                    {
                        if (prop.CanWrite)
                        {
                            cloneCollection = (origCollection as Array).Clone() as IList;
                            prop.SetValue(clone, cloneCollection, null);
                        }
                    }
                    else
                    {
                        if (cloneCollection == null)
                        {
                            cloneCollection = Activator.CreateInstance(t) as IList;
                            prop.SetValue(clone, cloneCollection, null);
                        }

                        foreach (object elem in origCollection)
                        {
                            cloneCollection.Add(Clone(elem, flavor, cloneDictionary));
                        }
                    }

                    continue;
                }

                if (!prop.CanWrite)
                    continue;

                if (prop.PropertyType.IsValueType)
                {
                    prop.SetValue(clone,
                                    prop.GetValue(model, null),
                                    null);
                }
                else
                {
                    prop.SetValue(clone,
                                    Clone(prop.GetValue(model, null), flavor, cloneDictionary),
                                    null);
                }
            }

            return clone;
        }


        private static bool ExcludeProperty(object model, string flavor, PropertyInfo prop)
        {
            bool excluded = false;
            object[] attributes = prop.GetCustomAttributes(typeof(CloneableAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                foreach (CloneableAttribute attrib in attributes)
                {
                    if (string.IsNullOrEmpty(attrib.Flavor))
                    {
                        if (attrib.State == CloneableState.Exclude)
                            excluded = true;
                    }
                    else
                    {
                        if (attrib.Flavor == flavor)
                        {
                            if (attrib.State == CloneableState.Include)
                            {
                                excluded = false;
                                break;
                            }
                        }
                    }
                }
            }

            return excluded;
        }



        #endregion
    }

    #region Documentation Tags
    /// <summary>
    ///     Defines the logic of cloning. CloneableState.Exclude tells the <see cref="ObjectCloner"/> to skip particular property from cloning. 
    ///     And CloneableState.Include forces <see cref="ObjectCloner"/> to copy the field too.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Class Information:
    ///	    <list type="bullet">
    ///         <item name="authors">Authors: Ruben Hakopian</item>
    ///         <item name="date">February 2009</item>
    ///         <item name="originalURL">http://www.rubenhak.com/?p=8</item>
    ///     </list>
    /// </para>
    /// </remarks>
    #endregion
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CloneableAttribute : Attribute
    {
        #region Fields

        private CloneableState _state;
        private string _flavor;

        #endregion

        #region Constructors

        public CloneableAttribute()
            : this(CloneableState.Include)
        {
        }

        public CloneableAttribute(CloneableState state)
            : this(state, string.Empty)
        {
        }

        public CloneableAttribute(CloneableState state, string flavor)
        {
            _state = state;
            _flavor = flavor;
        }

        #endregion

        #region Properties

        public CloneableState State
        {
            get { return _state; }
        }

        public string Flavor
        {
            get { return _flavor; }
        }

        #endregion
    }

    public enum CloneableState
    {
        Include,
        Exclude
    }
}
