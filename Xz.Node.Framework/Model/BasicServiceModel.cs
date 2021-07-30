using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Xz.Node.Framework.Model
{
    public abstract class BasicServiceModel<T> : BasicModel where T : new()
    {
        [JsonIgnore]
        //[ScriptIgnore]
        [Display(Name = "实体")]
        public T Service = new T();
    }
}
