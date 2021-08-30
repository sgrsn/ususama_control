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
      // テスト用
      move_seq_test.Add(new Pose2D(000f / 1000, 0000f / 1000, 0.00f, CleanState.Move));
      move_seq_test.Add(new Pose2D(000f / 1000, 0000f / 1000, 1.57f, CleanState.Move));
      move_seq_test.Add(new Pose2D(000f / 1000, 0000f / 1000, 3.14f, CleanState.Move));
      move_seq_test.Add(new Pose2D(000f / 1000, 0000f / 1000, 0.00f, CleanState.Move));
      move_seq_test.Add(new Pose2D(000f / 1000, 0000f / 1000, 1.57f, CleanState.Move));
      move_seq_test.Add(new Pose2D(000f / 1000, 0000f / 1000, 3.14f, CleanState.Move));
      //move_seq_test.Add(new Pose2D(0000f / 1000, 0000f / 1000, 0.00f, CleanState.Move));

      // seq1: ペーパ切れ端を吹き飛ばしておくタスク
      move_seq_1.Add(new Pose2D(0000f / 1000, 000f / 1000, 0.00f, CleanState.Move)); // 
      move_seq_1.Add(new Pose2D(1000f / 1000, 000f / 1000, 0.00f, CleanState.Move)); // まっすぐ進む 芯回収
      move_seq_1.Add(new Pose2D(1200f / 1000, 000f / 1000, 1.57f, CleanState.Move)); // 90deg 旋回
      move_seq_1.Add(new Pose2D(1200f / 1000, 700f / 1000, 1.57f, CleanState.Move)); // ゴミ箱に芯を入れる
      move_seq_1.Add(new Pose2D(1200f / 1000, 000f / 1000, 1.57f, CleanState.Move)); // 戻る

      // seq1: ゴミ1(仮に芯とする)をゴミ箱に入れるタスク
      move_seq_1.Add(new Pose2D(0000f / 1000, 000f / 1000, 0.00f, CleanState.Move)); // 
      move_seq_1.Add(new Pose2D(1000f / 1000, 000f / 1000, 0.00f, CleanState.Move)); // まっすぐ進む 芯回収
      move_seq_1.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move)); // 90deg 旋回
      move_seq_1.Add(new Pose2D(1200f / 1000, 700f / 1000, 1.57f, CleanState.Move)); // ゴミ箱に芯を入れる
      move_seq_1.Add(new Pose2D(1200f / 1000, 000f / 1000, 1.57f, CleanState.Move)); // 戻る

      // seq2: 便器前の床、便器を掃除するタスク
      move_seq_2.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move)); // トイレの前に移動
      move_seq_2.Add(new Pose2D(1200f / 1000, 450f / 1000, 3.14f, CleanState.Move)); // トイレ正面となるように旋回
      move_seq_2.Add(new Pose2D(1100f / 1000, 450f / 1000, 3.14f, CleanState.Clean));  // 床掃除

      // seq3: 床に散乱したごみをゴミ箱にいれるタスク
      move_seq_3.Add(new Pose2D(1200f / 1000, 450f / 1000, 3.14f, CleanState.Move)); // 便器から下がる
      move_seq_3.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move)); // 向きを変える
      move_seq_3.Add(new Pose2D(1200f / 1000, 000f / 1000, 1.57f, CleanState.Move)); // 下がる
      move_seq_3.Add(new Pose2D(1050f / 1000, 000f / 1000, 1.57f, CleanState.Move)); // 横にスライド
      move_seq_3.Add(new Pose2D(1050f / 1000, 700f / 1000, 1.57f, CleanState.Move)); // 掃除

      // seq4: ゴミ2(仮に紙コップとする)をゴミ箱に入れるタスク
      move_seq_4.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move)); // 後ろに下がる
      move_seq_4.Add(new Pose2D(1200f / 1000, 450f / 1000, 2.50f, CleanState.Move)); // 紙コップに向きを変更
      move_seq_4.Add(new Pose2D(1050f / 1000, 680f / 1000, 2.50f, CleanState.Move)); // 紙コップ回収
      move_seq_4.Add(new Pose2D(1050f / 1000, 650f / 1000, 1.57f, CleanState.Move)); // 紙コップ移動
      move_seq_4.Add(new Pose2D(1200f / 1000, 650f / 1000, 1.57f, CleanState.Move)); // 紙コップ横移動
      move_seq_4.Add(new Pose2D(1200f / 1000, 700f / 1000, 1.57f, CleanState.Move)); // 紙コップゴミ箱に入れる
      move_seq_4.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move)); // 戻る

      // seq5: ホームに戻るタスク
      //home_seq.Add(new Pose2D(0f, 0f, 0f, CleanState.Home)); // 
      //home_seq.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move)); 
      //home_seq.Add(new Pose2D(1000f / 1000, 000f / 1000, 0.00f, CleanState.Move));
      home_seq.Add(new Pose2D(1200f / 1000, 450f / 1000, 1.57f, CleanState.Move));
      home_seq.Add(new Pose2D(1200f / 1000, 100f / 1000, 1.57f, CleanState.Move));
      home_seq.Add(new Pose2D(1200f / 1000, 100f / 1000, 0.00f, CleanState.Move));
      home_seq.Add(new Pose2D(1200f / 1000, 000f / 1000, 0.00f, CleanState.Move));
      home_seq.Add(new Pose2D(0000f / 1000, 000f / 1000, 0.00f, CleanState.Move));
    }
  }
}