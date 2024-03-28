using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.TuringMachine
{
    public class State
    {

        /*Konstruktor*/
        public State(string name)
        {
            Name = name;
        }
        public string Name
        {
            get; set;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            State? other = obj as State;
            if (other == null)
            {
                return false;
            }
            else
            {
                return other.Name == Name;
            }
            
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
