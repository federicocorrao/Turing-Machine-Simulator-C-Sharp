using System;
using System.Collections.Generic;
using System.Text;

namespace TuringMachine
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Turing Machine Simulator v 0.1");

			Machine m = new Machine();
			m.Load("Program.txt");

			Simulator s = new Simulator(m);
			s.Initialize("abcdefghil*".ToCharArray(), 0);
	
			while (true)
			{
				Console.WriteLine("next: ");
				Console.ReadKey(true);
				s.Next();
			}
		}

		private static void Pause()
		{
			Console.Write("Premere un tasto per continuare... ");
			Console.ReadKey(true);
		}
	}

	public class Directive
	{
		public string estate;
		public char echar;
		public char move;
		public Directive(string es,char ec,char m)
		{
			estate = es;
            echar = ec;
            move = m;
		}
	}

	public class Machine
	{
		public char[] inputalphabet;	// alfabeto input
		public char[] outputalphabet;	// alfabeto output
		public string[] states;			// stati
		public string initstate;		// stato iniziale
		public string haltstate;		// stato terminatore
		
		public Dictionary<KeyValuePair<string, char>, Directive> Instructions // istruzioni: (q, c) -> (q, c, m)
			= new Dictionary<KeyValuePair<string,char>,Directive>();

		public void Load(string filename)
		{
			string[] lines = System.IO.File.ReadAllLines(filename);
			string[] toks;

			int linenumber = -1;
			foreach (string istr in lines)
			{
				linenumber++;
				if (istr[0] == '#') continue;
				
				toks = istr.Split(' ');
				switch (toks[0])
				{
					case "INPUT-ALPHABET": inputalphabet = toks[1].ToCharArray(); break;
					case "OUTPUT-ALPHABET": outputalphabet = toks[1].ToCharArray(); break;
					case "STATES":
						states = new string[toks.Length - 1];
						for (int i = 1; i < toks.Length; i++) states[i - 1] = toks[i];
						break;
					case "SSTATE": initstate = toks[1]; break;
					case "HALTS": haltstate = toks[1]; break;
					case "INSTRUCTIONS":
						for (int i = linenumber + 1; i < lines.Length; i++) // sulle linee delle istr
						{
							if (lines[i][0] == '#') continue; // salto i commenti

							// q, c, q, c, m
							string[] tks = lines[i].Split(new char[]{'>', ' '}, StringSplitOptions.RemoveEmptyEntries);

							if (tks[2] == haltstate) // terminazione
							{
								Instructions.Add(new KeyValuePair<string,char>(tks[0], tks[1][0]),
									new Directive(tks[2], '_', 'N'));
							}
							else
							{
								Instructions.Add(new KeyValuePair<string,char>(tks[0], tks[1][0]),
									new Directive(tks[2], tks[3][0], tks[4][0]));
							}
						}
						return;
				}
			}
		}
	}
	
	public class Simulator
	{
		Machine mdt;
		char[] Input;
		int index;
		string currentstate;

		public Simulator(Machine m) { mdt = m; }

		public void Initialize(char[] input, int start)
		{ Input = input; index = start; currentstate = mdt.initstate; }

		public void Next()
		{
			KeyValuePair<string, char> condition = new KeyValuePair<string, char>
				(currentstate, Input[index]);

			Directive d = mdt.Instructions[condition];
			if (d.estate == mdt.haltstate) return;

			currentstate = d.estate;
			Input[index] = d.echar;
			index += (d.move == 'R') ? 1 : -1;
			Console.WriteLine(currentstate + " " + index + " " + tos(Input) );
		}

		string tos(char[] chs)
		{
			string s = string.Empty;
			foreach (char c in chs) s += c;
			return s;
		}
	}
}
