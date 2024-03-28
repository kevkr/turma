using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using turma.Models.TuringMachine;
using turma.Views;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.Utilities;

namespace turma.ViewModels
{
    public class Serializer
    {
        public string serialize(TuringMachine turingMachine)
        {
            try {
                var StateConverter = new TuringMachineConverter();
                var yamlSerializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).WithTypeConverter(StateConverter).Build();
                return yamlSerializer.Serialize(turingMachine);
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return "";
            }
            
        }

        public TuringMachine? deserialize(string turingMachineDefinition)
        {
            try {
                var turingMachineConverter = new TuringMachineConverter();
                var yamlDeserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).WithTypeConverter(turingMachineConverter).Build();
                return yamlDeserializer.Deserialize<TuringMachine>(turingMachineDefinition);
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        /* Normalize Line Endings to CRLF */
        public string NormalizeLineEndings(string toNormalize)
        {
            return toNormalize.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
        } 

        public sealed class TuringMachineConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type)
            {
                return type == typeof(TuringMachine);
            }

            public object? ReadYaml(IParser parser, Type type)
            {
                try
                {
                    TuringMachine turingMachine = new TuringMachine();
                    parser.Consume<MappingStart>();

                    parser.Consume<Scalar>();
                    if (parser.TryConsume<SequenceStart>(out var _))
                    {
                        List<char> inputAlphabet = new List<char>();

                        while (parser.TryConsume<Scalar>(out var scalarItem))
                        {
                            inputAlphabet.Add(scalarItem.Value.ToCharArray()[0]);
                        }
                        parser.Consume<SequenceEnd>();
                        turingMachine.TapeAlphabet.InputAlphabet = inputAlphabet;
                    }

                    parser.Consume<Scalar>();
                    if (parser.TryConsume<SequenceStart>(out var _))
                    {
                        List<char> tapeAlphabet = new List<char>();

                        while (parser.TryConsume<Scalar>(out var scalarItem))
                        {
                            tapeAlphabet.Add(scalarItem.Value.ToCharArray()[0]);
                        }
                        parser.Consume<SequenceEnd>();
                        turingMachine.TapeAlphabet.TapeSymbols = tapeAlphabet;
                    }

                    parser.Consume<Scalar>();
                    if (parser.TryConsume<Scalar>(out var tapeCountScalar))
                    {
                        turingMachine.TapeCount = Int32.Parse(tapeCountScalar.Value);
                    }

                    parser.Consume<Scalar>();
                    if (parser.TryConsume<SequenceStart>(out var _))
                    {
                        List<State> stateSet = new List<State>();

                        while (parser.TryConsume<Scalar>(out var scalarItem))
                        {
                            stateSet.Add(new State(scalarItem.Value));
                        }
                        parser.Consume<SequenceEnd>();
                        turingMachine.StateSet.AllStates = stateSet;
                    }

                    if (parser.TryConsume<Scalar>(out var leftStateSetScalar))
                    {
                        if (leftStateSetScalar.Value == "leftStateSet")
                        {
                            if (parser.TryConsume<SequenceStart>(out var _))
                            {
                                List<State> leftStateSet = new List<State>();

                                while (parser.TryConsume<Scalar>(out var scalarItem))
                                {
                                    leftStateSet.Add(new State(scalarItem.Value));
                                }
                                parser.Consume<SequenceEnd>();
                                turingMachine.StateSet.LeftStates = leftStateSet;
                            }
                            parser.Consume<Scalar>();
                            if (parser.TryConsume<SequenceStart>(out var _))
                            {
                                List<State> rightStateSet = new List<State>();

                                while (parser.TryConsume<Scalar>(out var scalarItem))
                                {
                                    rightStateSet.Add(new State(scalarItem.Value));
                                }
                                parser.Consume<SequenceEnd>();
                                turingMachine.StateSet.RightStates = rightStateSet;
                            }
                            parser.Consume<Scalar>();
                        }
                    };

                    if (parser.TryConsume<Scalar>(out var startingStateScalar))
                    {
                        if (startingStateScalar.Value.Length > 0)
                        {
                            turingMachine.StateSet.StartingState = new State(startingStateScalar.Value);
                        }
                    }

                    parser.Consume<Scalar>();
                    if (parser.TryConsume<SequenceStart>(out var _))
                    {
                        List<State> acceptingStates = new List<State>();

                        while (parser.TryConsume<Scalar>(out var scalarItem))
                        {
                            acceptingStates.Add(new State(scalarItem.Value));
                        }
                        parser.Consume<SequenceEnd>();
                        turingMachine.StateSet.AcceptingStates = acceptingStates;
                    }

                    parser.Consume<Scalar>();
                    if (parser.TryConsume<SequenceStart>(out var _))
                    {
                        List<Transition> transitions = new List<Transition>();

                        while (parser.TryConsume<SequenceStart>(out var _))
                        {
                            Transition t = new Transition();
                            if (parser.TryConsume<Scalar>(out var conditionStateItem))
                            {
                                t.ConditionState = new State(conditionStateItem.Value);
                            }
                            if (parser.TryConsume<SequenceStart>(out var _))
                            {
                                List<char> conditionTapeSymbols = new List<char>();
                                while (parser.TryConsume<Scalar>(out var conditionTapeSymbolItem))
                                {
                                    conditionTapeSymbols.Add(conditionTapeSymbolItem.Value.ToCharArray()[0]);
                                }
                                parser.Consume<SequenceEnd>();
                                t.ConditionTapeSymbol = conditionTapeSymbols.ToArray();
                            }
                            if (parser.TryConsume<Scalar>(out var instructionStateItem))
                            {
                                t.InstructionState = new State(instructionStateItem.Value);
                            }
                            if (parser.TryConsume<SequenceStart>(out var _))
                            {
                                List<char> instructionTapeSymbols = new List<char>();
                                while (parser.TryConsume<Scalar>(out var conditionTapeSymbolItem))
                                {
                                    instructionTapeSymbols.Add(conditionTapeSymbolItem.Value.ToCharArray()[0]);
                                }
                                parser.Consume<SequenceEnd>();
                                t.InstructionTapeSymbol = instructionTapeSymbols.ToArray();
                            }
                            if (parser.TryConsume<SequenceStart>(out var _))
                            {
                                List<Transition.Direction> directions = new List<Transition.Direction>();
                                while (parser.TryConsume<Scalar>(out var directionItem))
                                {
                                    Enum.TryParse(directionItem.Value, out Transition.Direction d);
                                    directions.Add(d);
                                }
                                parser.Consume<SequenceEnd>();
                                t.Directions = directions.ToArray();
                            }
                            if (parser.TryConsume<Scalar>(out var descriptionItem))
                            {
                                t.Description = descriptionItem.Value;
                            }
                            else
                            {
                                t.Description = "";
                            }
                            transitions.Add(t);
                            parser.Consume<SequenceEnd>();
                        }
                        parser.Consume<SequenceEnd>();
                        turingMachine.Transitions = transitions;
                    }


                    parser.Consume<MappingEnd>();
                    return turingMachine;
                } catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return null;
                }
            }

            public void WriteYaml(IEmitter emitter, object? value, Type type)
            {
                TuringMachine? turingMachine = value as TuringMachine;

                if (turingMachine == null) return;

                try { 

                emitter.Emit(new MappingStart());
                
                emitter.Emit(new Scalar("inputAlphabet"));
                emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                foreach (char c in turingMachine.TapeAlphabet.InputAlphabet)
                {
                    emitter.Emit(new Scalar(c.ToString()));
                }
                emitter.Emit(new SequenceEnd());

                emitter.Emit(new Scalar("tapeAlphabet"));
                emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                foreach (char c in turingMachine.TapeAlphabet.TapeSymbols)
                {
                    emitter.Emit(new Scalar(c.ToString()));
                }
                emitter.Emit(new SequenceEnd());

                emitter.Emit(new Scalar("tapeCount"));
                emitter.Emit(new Scalar(turingMachine.TapeCount.ToString()));

                emitter.Emit(new Scalar("stateSet"));
                emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                foreach (State s in turingMachine.StateSet.AllStates)
                {
                    emitter.Emit(new Scalar(s.Name));
                }
                emitter.Emit(new SequenceEnd());

                if(turingMachine.StateSet.LeftStates != null && turingMachine.StateSet.RightStates != null && (turingMachine.StateSet.LeftStates.Count > 0 || turingMachine.StateSet.RightStates.Count > 0))
                {
                    emitter.Emit(new Scalar("leftStateSet"));
                    emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                    foreach (State s in turingMachine.StateSet.LeftStates)
                    {
                        emitter.Emit(new Scalar(s.Name));
                    }
                    emitter.Emit(new SequenceEnd());

                    emitter.Emit(new Scalar("rightStateSet"));
                    emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                    foreach (State s in turingMachine.StateSet.RightStates)
                    {
                        emitter.Emit(new Scalar(s.Name));
                    }
                    emitter.Emit(new SequenceEnd());
                }

                emitter.Emit(new Scalar("startingState"));
                if (turingMachine.StateSet.StartingState != null)
                {
                    emitter.Emit(new Scalar(turingMachine.StateSet.StartingState.Name));
                }
                else
                {
                    emitter.Emit(new Scalar(""));
                }

                emitter.Emit(new Scalar("acceptingStates"));
                emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                foreach (State s in turingMachine.StateSet.AcceptingStates)
                {
                    emitter.Emit(new Scalar(s.Name));
                }
                emitter.Emit(new SequenceEnd());

                emitter.Emit(new Scalar("transitions"));
                emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Block));
                foreach (Transition t in turingMachine.Transitions)
                {
                    emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));

                    if(t.ConditionState != null)
                    {
                        emitter.Emit(new Scalar(t.ConditionState.Name));
                    }

                    emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                    foreach (char c in t.ConditionTapeSymbol)
                    {
                        emitter.Emit(new Scalar(c.ToString()));
                    }
                    emitter.Emit(new SequenceEnd());

                    if (t.InstructionState != null)
                    {
                        emitter.Emit(new Scalar(t.InstructionState.Name));
                    }

                    emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                    foreach (char c in t.InstructionTapeSymbol)
                    {
                        emitter.Emit(new Scalar(c.ToString()));
                    }
                    emitter.Emit(new SequenceEnd());

                    emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                    foreach (Transition.Direction d in t.Directions)
                    {
                        emitter.Emit(new Scalar(d.ToString()));
                    }
                    emitter.Emit(new SequenceEnd());
                    if (t.Description != null)
                    {
                        emitter.Emit(new Scalar(t.Description));
                    }
                    else
                    {
                        emitter.Emit(new Scalar(""));
                    }
                    emitter.Emit(new SequenceEnd());
                }
                emitter.Emit(new SequenceEnd());
                emitter.Emit(new MappingEnd());
                } catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    MessageBox.Show(App.MainWindow, "Fehler beim Serialisieren der Turing Maschine.", "Error", MessageBox.MessageBoxButtons.Ok);
                }
            }
        }
    }
}
