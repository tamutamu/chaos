using System;
using System.Collections.Generic;
using System.Text;

using Yanesdk.Draw;
using Yanesdk.Input;

namespace Yanesdk.GUIParts
{
	/// <summary>
	/// テクスチャを描画するだけの描画コントロール
	/// </summary>
	public class TextureImage : ITextureGUI , IDisposable
	{

		/// <summary>
		/// defFile : TextureLoaderの定義ファイル
		/// isDefFileRelative = trueにすれば
		/// 定義ファイル相対で、定義ファイルに書かれているファイルを
		/// 読み込むことが出来る。
		/// </summary>
		/// <param name="defFile"></param>
		/// <param name="isDefFileRelative"></param>
		public TextureImage(string defFile , bool isDefFileRelative)
		{
			this.defFile = defFile;
			this.isDefFileRelative = isDefFileRelative;
		}
		bool isDefFileRelative;
		private string defFile;
		private string imageFileName;

		/// <summary>
		/// SmartTextureLoaderと読み込みたい画像ファイル名を渡す場合のコンストラクタ
		/// </summary>
		/// <param name="sc"></param>
		/// <param name="filename"></param>
		public TextureImage(string imageFileName)
		{
			this.imageFileName = imageFileName;
		}

		public virtual void OnInit(ControlContext cc)
		{
			if ( imageFileName != null )
			{
				loader = cc.SmartTextureLoader.LoadDefFile("mem:"+imageFileName);
			}
			else
			{
				loader = cc.SmartTextureLoader.LoadDefFile(defFile
					,isDefFileRelative);
			}

			/*
			// これ、ここでやってしまうと、コンテキスト選択されてないとおかしゅうなる。

			ITexture t = loader.GetTexture(0);
			// ボタンサイズを取得しておき、これをマウスの判定矩形として利用する
			if (t != null)
			{
				width = (int)t.Width;
				height = (int)t.Height;
			}
			 */
		}

		/// <summary>
		/// ボタンの色を設定/取得する
		/// (半透明にしたいときなどに設定して)
		/// </summary>
		public Color4ub? Color
		{
			get { return color; }
			set { color = value; }
		}
		private Color4ub? color;

		public virtual void OnPaint(IScreen scr , ControlContext cc , int x , int y)
		{
			if (!visible) return ;

			ITexture tex = loader.GetTexture(loaderOffset);
			if ( tex != null )
			{
				if ( color != null )
				{
					scr.SetColor(color.Value);
					scr.Blt(tex , x , y);
					scr.ResetColor();
				}
				else
				{
					scr.Blt(tex , x , y);
				}
			}
		}

		private TextureLoader loader;

		/// <summary>
		/// loaderに、表示するときのoffsetを指定できる。
		/// defaultでは0。たとえば、この数値として5を指定すれば、
		/// Disable時に描画されるテクスチャ番号は、本来の3から、3+5 = 8になる。
		/// </summary>
		public int LoaderOffset
		{
			get { return loaderOffset; }
			set { loaderOffset = value; }
		}
		private int loaderOffset;


		/// <summary>
		/// このボタンは表示されているか？
		/// Visible = falseにすると、何も表示されない。
		/// 
		/// defaultでは、true
		/// </summary>
		public bool Visible
		{
			get { return visible; }
			set { visible = value; }
		}
		private bool visible = true;

		public void Dispose()
		{
		//	loader.Dispose();
		// cacheしているのであえて解放しない
		}
	}
}
