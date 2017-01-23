using Polly;

namespace WebApi.Policies
{
    public class GroupPolicyBuilder
    {
        private readonly PolicyRegistry _policyRegistry;

        public GroupPolicyBuilder(PolicyRegistry policyRegistry)
        {
            _policyRegistry = policyRegistry;
        }
        private string _policyKey;

        public GroupPolicyBuilder Using(string policyKey)
        {
            AppendKey(policyKey);
            return this;
        }

        private void AppendKey(string policyKey)
        {
            if (_policyKey != null)
                _policyKey += "|";
            _policyKey += policyKey;
        }

        public Policy GetPolicy()
        {
            return _policyRegistry.Get(_policyKey);
        }
    }
}