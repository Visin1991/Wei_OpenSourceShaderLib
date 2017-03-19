namespace Visin1_1
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    class BindToUpdate : System.Attribute
    {
        public string _description;
        public BindToUpdate(string description)
        {
            _description = description;
        }

        public BindToUpdate()
        {
            _description = "Haha Suck";
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    class BindToStart : System.Attribute
    {
        public string _description;
        public BindToStart(string description)
        {
            _description = description;
        }

        public BindToStart()
        {
            _description = "Haha Suck";
        }
    }    
}

/// Attribute
///     AttributeUsage
///         ***attribute class itself is marked with an attribute—the System.AttributeUsage attribute.
///         ***AttributeUsage only applay to attribute. it is more like a meta-Attribute
///         
///         [System.AttributeUsage(System.AttributeTargets.All,AllowMultiple = false,Inherited = true)] 
///         class NewAttribute : System.Attribute { }  
///         
///         ***The AllowMultiple and Inherited arguments are optional. and defult values are false,true
///         
///         ***The first AttributeUsage argument must be one or more elements of the AttributeTargets enumeration. Multiple target types can be linked together with the OR operator, like this:    
/// 
///         [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]  
///         class NewPropertyOrFieldAttribute : Attribute { }
///         
///         Target Type:
///                 ➤➤ All
///                 ➤➤ Assembly
///                 ➤➤ Class
///                 ➤➤ Constructor
///                 ➤➤ Delegate
///                 ➤➤ Enum
///                 ➤➤ Event
///                 ➤➤ Field
///                 ➤➤ GenericParameter
///                 ➤➤ Interface
///                 ➤➤ Method
///                 ➤➤ Module
///                 ➤➤ Parameter
///                 ➤➤ Property
///                 ➤➤ ReturnValue
///                 ➤➤ Struct
///
///          1. We first define A Attribute with AttributeUsage
///                 specify type by property 
///                 specity parameter by  Constructor.
///          
///          2. Use the Attribute apply the a Object....... pretty much we can do anything we want.
///                 save a function pointer.
///                 save a object instance
///                 ......
///