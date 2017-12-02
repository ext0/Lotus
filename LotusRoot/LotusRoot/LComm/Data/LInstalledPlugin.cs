using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    [Serializable()]
    public class LInstalledPlugin
    {
        private String _name;
        private String _description;
        private String _author;
        private int _version;
        private String _classPathNameData;
        private byte[] _classData;

        private bool _enabled;

        public LInstalledPlugin(String name, String description, String author, int version, String classPathNameData, byte[] classData, bool enabled)
        {
            _name = name;
            _description = description;
            _author = author;
            _version = version;
            _classPathNameData = classPathNameData;
            _classData = classData;
            _enabled = enabled;
        }

        public String Name
        {
            get
            {
                return _name;
            }
        }

        public String Description
        {
            get
            {
                return _description;
            }
        }

        public String Author
        {
            get
            {
                return _author;
            }
        }

        public int Version
        {
            get
            {
                return _version;
            }
        }

        public String ClassPathNameData
        {
            get
            {
                return _classPathNameData;
            }
        }

        public byte[] ClassData
        {
            get
            {
                return _classData;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is LInstalledPlugin)
            {
                LInstalledPlugin plugin = (obj as LInstalledPlugin);
                return plugin.Name.Equals(Name);
            }
            return false;
        }
    }
}
