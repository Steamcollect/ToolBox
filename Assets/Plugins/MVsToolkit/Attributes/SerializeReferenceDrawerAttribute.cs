using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MVsToolkit.Dev
{
    /// <summary>
    /// Provides a custom drawer for fields marked with [SerializeReference].
    /// <para>
    /// Use this attribute to enhance the Inspector display of polymorphic serialized references.
    /// </para>
    /// <para>
    /// Displays the actual type of the referenced object and allows changing it through a dropdown.
    /// </para>
    /// <para>Example:</para>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///       <code>[SerializeReference, SerializeReferenceDrawer]</code> → Shows type selector in Inspector
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       <code>public IWeapon weapon;</code> → Can select any class implementing IWeapon
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeReferenceDrawerAttribute : PropertyAttribute{}
}