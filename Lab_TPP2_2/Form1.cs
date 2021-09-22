using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_TPP2_2
{
    public partial class Form1 : Form
    {
        private Assembly assembly;
        public Form1()
        {
            InitializeComponent();
        }
        private void openAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            string path = selectAssemblyFile();
            if (path != null)
            {
                assembly = openAssembly(path);
            }
            if (assembly != null)
            {
                TreeNode root = new TreeNode();
                root.Text = assembly.GetName().Name;
                root.ImageIndex = 0;
                root.SelectedImageIndex = 0;
                treeView1.Nodes.Add(root);
                Type[] types = assembly.GetTypes();
                addRoot(root, types);
            }
        }
        private string selectAssemblyFile()
        {
            openFileDialog1.Filter = "Dll files (*.dll)|*.dll|Exe files (*.exe) | *.exe | All files(*.*) | *.* ";
            openFileDialog1.Title = "Select an assembly file";
            return (openFileDialog1.ShowDialog() == DialogResult.OK) ?
            openFileDialog1.FileName : null;
        }
        private Assembly openAssembly(string path)
        {
            try
            {
                Assembly a = Assembly.LoadFrom(path);
                return a;
            }
            catch (Exception)
            {
                MessageBox.Show("Error on loading the assembly!",
                "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        void addRoot(TreeNode root, Type[] types)
        {
            TreeNode node = null;
            foreach (Type type in types)
            {
                node = new TreeNode();
                node.Text = type.ToString();
                if (type.IsClass)
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    addFirstLevel(node, type);
                    root.Nodes.Add(node);
                }
                else if (type.IsInterface)
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                    addFirstLevel(node, type);
                    root.Nodes.Add(node);
                }
                else if (type.IsEnum)
                {
                    node.ImageIndex = 3;
                    node.SelectedImageIndex = 3;
                    addFirstLevel(node, type);
                    root.Nodes.Add(node);
                }
            }
        }
        private void addFirstLevel(TreeNode node, Type type)
        {
            TreeNode node1 = null;
            FieldInfo[] fields = type.GetFields();
            FieldInfo[] privatefields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo[] methods = type.GetMethods();
            MethodInfo[] privatemethods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo[] privateconstructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                node1 = new TreeNode();
                node1.Text = field.FieldType.Name + " " + field.Name;
                node1.ImageIndex = 5;
                node1.SelectedImageIndex = 5;
                node.Nodes.Add(node1);
            }
            foreach (FieldInfo field in privatefields)
            {
                node1 = new TreeNode();
                node1.Text = "private " + field.FieldType.Name + " " + field.Name;
                node1.ImageIndex = 5;
                node1.SelectedImageIndex = 5;
                node.Nodes.Add(node1);
            }
            foreach (ConstructorInfo constructor in constructors)
            {
                string s = "";
                ParameterInfo[] parametrs = constructor.GetParameters();
                foreach (ParameterInfo parametr in parametrs)
                {
                    s = s + parametr.ParameterType.Name + ", ";
                }
                s = s.Trim();
                s = s.TrimEnd(',');
                node1 = new TreeNode();
                node1.Text = node.Text + "(" + s + ")";
                node1.ImageIndex = 6;
                node1.SelectedImageIndex = 6;
                node.Nodes.Add(node1);
            }
            foreach (ConstructorInfo constructor in privateconstructors)
            {
                string s = "private ";
                ParameterInfo[] parametrs = constructor.GetParameters();
                foreach (ParameterInfo parametr in parametrs)
                {
                    s = s + parametr.ParameterType.Name + ", ";
                }
                s = s.Trim();
                s = s.TrimEnd(',');
                node1 = new TreeNode();
                node1.Text = node.Text + "(" + s + ")";
                node1.ImageIndex = 6;
                node1.SelectedImageIndex = 6;
                node.Nodes.Add(node1);
            }
            foreach (MethodInfo method in methods)
            {
                string s = "";
                ParameterInfo[] parametrs = method.GetParameters();
                foreach (ParameterInfo parametr in parametrs)
                {
                    s = s + parametr.ParameterType.Name + ", ";
                }
                s = s.Trim();
                s = s.TrimEnd(',');
                node1 = new TreeNode();
                node1.Text = method.ReturnType.Name + " " + method.Name + "(" + s + ")";
                node1.ImageIndex = 4;
                node1.SelectedImageIndex = 4;
                node.Nodes.Add(node1);
            }
            foreach (MethodInfo method in privatemethods)
            {
                string s = "";
                ParameterInfo[] parametrs = method.GetParameters();
                foreach (ParameterInfo parametr in parametrs)
                {
                    s = s + "private " + parametr.ParameterType.Name + ", ";
                }
                s = s.Trim();
                s = s.TrimEnd(',');
                node1 = new TreeNode();
                node1.Text = method.ReturnType.Name + " " + method.Name + "(" + s + ")";
                node1.ImageIndex = 4;
                node1.SelectedImageIndex = 4;
                node.Nodes.Add(node1);
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            listView1.Clear();

            switch (treeView1.SelectedNode.Level)
            {
                case 0:
                    listView1.Items.Add(" Assembly\n");
                    listView1.Items.Add(" Name - " + assembly.GetName().Name);
                    break;
                case 1:
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.ToString() == node.Text)
                        {
                            if (type.IsClass)
                            {
                                Type[] itfcs = type.GetInterfaces();
                                listView1.Items.Add("Class");
                                listView1.Items.Add(" Name: " + type.FullName);
                                string modificator = type.IsNotPublic ? "private" :
                                                     type.IsPublic ? "public" : "undefined";
                                listView1.Items.Add(" Access: " + modificator);
                                listView1.Items.Add(" Is Abstract: " + type.IsAbstract);
                                listView1.Items.Add(" Is Sealed: " + type.IsSealed);
                                listView1.Items.Add(" Inherited from: " + type.BaseType.Name);
                                listView1.Items.Add(" Implements interfaces: ");
                                foreach (var itfc in itfcs)
                                    listView1.Items.Add("    " + itfc.Name);
                            }
                            else if (type.IsInterface)
                            {
                                listView1.Items.Add("Interface");
                                listView1.Items.Add(" Name: " + type.FullName);
                                string modificator = type.IsNotPublic ? "private" :
                                                     type.IsPublic ? "public" : "undefined";
                                listView1.Items.Add(" Access: " + modificator);
                            }
                            else if (type.IsEnum)
                            {
                                listView1.Items.Add("Enumeration");
                                listView1.Items.Add(" Name: " + type.FullName);
                                string modificator = type.IsNotPublic ? "private" :
                                                     type.IsPublic ? "public" : "undefined";
                                listView1.Items.Add(" Access: " + modificator);
                            }
                        }
                    }
                    break;
                case 2:
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.ToString() == node.Parent.Text)
                        {
                            var fields = type.GetRuntimeFields();
                            MethodInfo[] methods = type.GetMethods();
                            ConstructorInfo[] constructors = type.GetConstructors();
                            string elemName = node.Text.Split('(').First().Split(' ').Last();

                            foreach (var field in fields)
                            {
                                if (field.Name == elemName)
                                {
                                    listView1.Items.Add("Field characteristics");
                                    listView1.Items.Add(" Name: " + field.Name);
                                    string modificator = field.IsPrivate ? "private" :
                                                         field.IsPublic ? "public" : "undefined";
                                    listView1.Items.Add(" Access: " + modificator);

                                    listView1.Items.Add("Primitive");
                                }
                            }
                            foreach (var constructor in constructors)
                            {
                                if (constructor.Name == elemName)
                                {
                                    listView1.Items.Add("Constructor characteristics");
                                    listView1.Items.Add(" Name: " + constructor.Name);
                                    string modificator = constructor.IsPrivate ? "private" :
                                                         constructor.IsPublic ? "public" : "undefined";
                                    listView1.Items.Add(" Access: " + modificator);

                                    listView1.Items.Add(" Constructor parametrs: ");
                                    foreach (var param in constructor.GetParameters())
                                        listView1.Items.Add("  " + param.Name);
                                }
                            }
                            foreach (var method in methods)
                            {
                                if (method.Name == elemName)
                                {
                                    listView1.Items.Add("Method characteristics");
                                    listView1.Items.Add(" Type: Method");
                                    listView1.Items.Add(" Name: " + method.Name);
                                    string modificator = method.IsPrivate ? "private" :
                                                         method.IsPublic ? "public" : "undefined";
                                    listView1.Items.Add(" Access: " + modificator);
                                    listView1.Items.Add(" Method parametrs: ");
                                    foreach (var param in method.GetParameters())
                                        listView1.Items.Add("  " + param.Name);
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
