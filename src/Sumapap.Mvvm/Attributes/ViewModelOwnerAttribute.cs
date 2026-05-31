namespace Sumapap.Mvvm.Attributes
{
    /// <summary>
    /// Specifies the type of view model associated with a class for use in view model binding scenarios.
    /// </summary>
    /// <remarks>Apply this attribute to a class to indicate which view model type it is intended to work
    /// with. This is commonly used in frameworks that support automatic view model resolution or dependency injection
    /// for views.</remarks>
    /// <param name="viewModelType">The type of the view model to associate with the attributed class. This type must not be null.</param>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute(Type viewModelType) : Attribute
    {
        public Type ViewModelType { get; } = viewModelType;

        public bool IsDefaultConstructor { get; init; } = false;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ViewModelOwnerAttribute<TViewModel> : ViewModelOwnerAttribute
    {
        public ViewModelOwnerAttribute()
            : base(typeof(TViewModel))
        {
        }
    }
}
