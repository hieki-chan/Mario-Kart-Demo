using UnityEngine;

namespace KartDemo.MVP
{
    public abstract class Presenter<V, M> /*where V : View where M : Model*/
    {
        [SerializeField] protected V view;
        [SerializeField] protected M model;

        //public Presenter(V view, M model)
        //{
        //    this.view = view;
        //    this.model = model;
        //}

        public abstract void Init();
    }
}