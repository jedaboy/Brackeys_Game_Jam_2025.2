namespace GRD.FSM
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class FSM_BehaviourAttribute : System.Attribute
    {
        private string path;

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public FSM_BehaviourAttribute(string path)
        {
            this.path = path;
        }
    }
}
