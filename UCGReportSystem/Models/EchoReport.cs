using System.ComponentModel.DataAnnotations;
using UCGReportSystem.Models;

public class EchoReport
{
    [Key]
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime ExamDate { get; set; }
    public string Technician { get; set; } = string.Empty;
    public string Physician { get; set; } = string.Empty;

    // 心臓サイズ測定値
    public double? AoD { get; set; } // 大動脈径
    public double? LAD { get; set; } // 左房径
    public double? LVIDd { get; set; } // 左室拡張末期径
    public double? LVIDs { get; set; } // 左室収縮末期径
    public double? IVSTd { get; set; } // 心室中隔壁厚
    public double? LVPWTd { get; set; } // 左室後壁厚
    public double? IVD { get; set; } // 

    // EF関連
    public double? TeichholtzEF { get; set; }
    public double? PomboEF { get; set; }
    public double? SimpsonEF { get; set; }
    public double? FS { get; set; } // 左室内径短縮率
    public bool LVH { get; set; } // 左室肥大
    public bool ASH { get; set; } // 非対称性中隔肥大

    // MV flow
    public double? E { get; set; } // E波
    public double? A { get; set; } // A波
    public double? E_A { get; set; } // E/A比
    public double? DecT { get; set; } // 減衰時間
    public double? E_Eprime { get; set; } // E/e'比

    // 弁膜評価
    public bool MVP { get; set; } // 僧帽弁逸脱
    public string MVPDetails { get; set; } = string.Empty;
    public bool MVCalc { get; set; } // 僧帽弁石灰化
    public string MVCalcDetails { get; set; } = string.Empty;
    public bool MS { get; set; } // 僧帽弁狭窄
    public bool Doomine { get; set; } // Doomine
    public double? MVAByPHT { get; set; } // 僧帽弁口面積 by PHT
    public double? MVAByPlanimetry { get; set; } // 僧帽弁口面積 by Planimetry

    // 大動脈弁評価
    public bool AVCalc { get; set; } // 大動脈弁石灰化
    public string AVCalcDetails { get; set; } = string.Empty;
    public bool AS { get; set; } // 大動脈弁狭窄
    public double? PVE { get; set; } // 肺静脈血流
    public double? AVAByPlanimetry { get; set; } // 大動脈弁口面積 by Planimetry
    public double? AVAByEquation { get; set; } // 大動脈弁口面積 by 連続の式
    public double? MeanPG { get; set; } // 平均圧較差
    public double? MaxPG { get; set; } // 最大圧較差

    // 逆流評価（文字列として保存）
    public string MRGrade { get; set; } = string.Empty;
    public string ARGrade { get; set; } = string.Empty;
    public string TRGrade { get; set; } = string.Empty;
    public string PRGrade { get; set; } = string.Empty;
    public double? PHT { get; set; } // 圧半減時間
    public double? PG { get; set; } // 圧較差

    // 壁運動
    public bool Asynergy { get; set; } // 非同期性
    public string AsynergyDetails { get; set; } = string.Empty;

    // その他
    public bool PE { get; set; } // 心嚢液
    public string PEDetails { get; set; } = string.Empty;
    public bool Shunt { get; set; } // シャント
    public string ShuntDetails { get; set; } = string.Empty;

    // 所見と診断
    public string Findings { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;

    // 関連する患者
    public virtual Patient? Patient { get; set; }
}