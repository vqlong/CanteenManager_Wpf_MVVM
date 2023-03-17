using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public abstract class Model
    {
        public virtual int Id { get; set; }
        public virtual T GetClone<T>() where T : Model
        {
            if (GetType() == typeof(T))
                return (T)MemberwiseClone();
            else
                throw new Exception($"T phải là {GetType()}");
        }
        //public override string ToString()
        //{
        //    return base.ToString() + $" ~Id: {Id} {GetHashCode()}";
        //}
    }
}
