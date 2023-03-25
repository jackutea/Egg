using System.Collections.Generic;

namespace TiedanSouls.Client.Entities {

    public class DamageArbitService {

        Dictionary<ulong, List<DamageRecordModel>> all;

        public DamageArbitService() {
            all = new Dictionary<ulong, List<DamageRecordModel>>();
        }

    }

}