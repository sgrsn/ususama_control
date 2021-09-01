using System;
using System.Collections.Generic;

namespace ususama_routes
{
  public enum CleanState
  {
    Move, // 台車の移動
    Stop, // 台車の停止
    Clean,// アームによる掃除
    Home, // ホームに戻る
  };
  public class Pose2D
  {
    public float x;
    public float y;
    public float theta;
    public CleanState state;
    public Pose2D(float x, float y, float theta, CleanState state)
    {
      this.x = x;
      this.y = y;
      this.theta = theta;
      this.state = state;
    }
  };
  public class UsusamaRoutes
  {
    public List<Pose2D> move_seq_test = new List<Pose2D>();
    public List<Pose2D> move_seq_1 = new List<Pose2D>();
    public List<Pose2D> move_seq_2 = new List<Pose2D>();
    public List<Pose2D> move_seq_3 = new List<Pose2D>();
    public List<Pose2D> move_seq_4 = new List<Pose2D>();
    public List<Pose2D> move_seq_5 = new List<Pose2D>();
    public List<Pose2D> move_seq_6 = new List<Pose2D>();
    public List<Pose2D> move_seq_7 = new List<Pose2D>();
    public List<Pose2D> home_seq = new List<Pose2D>();
    public UsusamaRoutes()
    {
      // seq1: ゴミ1(仮に芯とする)をゴミ箱に入れるタスク
      move_seq_1.Add(new Pose2D(0000f / 1000, 000f / 1000, 0.00f, CleanState.Move)); // ホームポジション
      move_seq_1.Add(new Pose2D(1250f / 1000, 000f / 1000, 0.00f, CleanState.Move)); // まっすぐ進む 芯回収
      move_seq_1.Add(new Pose2D(1250f / 1000, 000f / 1000, 1.57f, CleanState.Move)); // 90deg 旋回

      // seq1: ゴミ1(仮に芯とする)をゴミ箱に入れるタスク
      move_seq_1.Add(new Pose2D(0000f / 1000, 000f / 1000, 0.00f, CleanState.Move)); // ホームポジション
      move_seq_1.Add(new Pose2D(1250f / 1000, 000f / 1000, 0.00f, CleanState.Move)); // まっすぐ進む 芯回収
      move_seq_1.Add(new Pose2D(1250f / 1000, 000f / 1000, 1.57f, CleanState.Move)); // 90deg 旋回

      // ここでファンをオン

      move_seq_2.Add(new Pose2D(1050f / 1000, 150f / 1000, 1.57f, CleanState.Move)); // 少し吹き飛ばす
      move_seq_2.Add(new Pose2D(1200f / 1000, 150f / 1000, 1.57f, CleanState.Move)); // 水にあたらないようスライド
      move_seq_2.Add(new Pose2D(1200f / 1000, 850f / 1000, 1.57f, CleanState.Move)); // ゴミ箱に入れる

      // seq3: 便器前の床、便器を掃除するタスク
      move_seq_3.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move)); // トイレの前に移動
      move_seq_3.Add(new Pose2D(1200f / 1000, 450f / 1000, 3.14f, CleanState.Move)); // トイレ正面となるように旋回

      // ここで床掃除、便器掃除

      // seq4: ゴミ2(仮に紙コップとする)をゴミ箱に入れるタスク
      move_seq_4.Add(new Pose2D(1200f / 1000, 450f / 1000, 3.14f, CleanState.Move)); // 近づく
      move_seq_4.Add(new Pose2D(1200f / 1000, 450f / 1000, 2.50f, CleanState.Move)); // 紙コップに向きを変更
      move_seq_4.Add(new Pose2D( 950f / 1000, 780f / 1000, 2.50f, CleanState.Move)); // 紙コップ回収
      move_seq_4.Add(new Pose2D( 950f / 1000, 780f / 1000, 1.57f, CleanState.Move)); // 姿勢をゴミ箱に垂直に変更
      move_seq_4.Add(new Pose2D(1200f / 1000, 650f / 1000, 1.57f, CleanState.Move)); // 紙コップ横移動
      move_seq_4.Add(new Pose2D(1200f / 1000, 750f / 1000, 1.57f, CleanState.Move)); // 紙コップゴミ箱に入れる
      move_seq_4.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move)); // 戻る

      // seq5: ホームに戻るタスク
      home_seq.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move));
      home_seq.Add(new Pose2D(1100f / 1000, 100f / 1000, 1.57f, CleanState.Move));
      home_seq.Add(new Pose2D(1100f / 1000, 100f / 1000, 0.00f, CleanState.Move));
      home_seq.Add(new Pose2D(1100f / 1000, 000f / 1000, 0.00f, CleanState.Move));
      home_seq.Add(new Pose2D(0000f / 1000, 000f / 1000, 0.00f, CleanState.Move));
    }
  }
}