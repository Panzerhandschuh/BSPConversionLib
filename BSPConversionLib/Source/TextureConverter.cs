﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using LibBSP;
using SharpCompress.Archives.Zip;
using SharpCompress.Archives;

namespace BSPConversionLib
{
	public class TextureConverter
	{
		private string pk3Dir;
		private BSP bsp;
		private string outputDir;

		private readonly HashSet<string> validImageExtensions = new HashSet<string>()
		{
			".bmp",
			".jpg",
			".jpeg",
			".png",
			".tga"
		};

		public TextureConverter(string pk3Dir, BSP bsp)
		{
			this.pk3Dir = pk3Dir;
			this.bsp = bsp;
		}

		public TextureConverter(string pk3Dir, string outputDir)
		{
			this.pk3Dir = pk3Dir;
			this.outputDir = outputDir;
		}

		public void Convert()
		{
			ConvertImagesToVtfs();
			MoveOrEmbedVtfs();
		}

		// TODO: Make multi-threaded
		private void ConvertImagesToVtfs()
		{
			var imageConverter = new ImageConverter();
			var files = Directory.GetFiles(pk3Dir, "*.*", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				if (validImageExtensions.Contains(Path.GetExtension(file)))
					imageConverter.Convert(file);
			}
		}

		private void MoveOrEmbedVtfs()
		{
			// TODO: Find textures using shader texture paths
			var vtfFiles = Directory.GetFiles(pk3Dir, "*.vtf", SearchOption.AllDirectories);
			var vmtFiles = Directory.GetFiles(pk3Dir, "*.vmt", SearchOption.AllDirectories);
			var textureFiles = vtfFiles.Concat(vmtFiles);

			if (bsp != null)
				EmbedFiles(textureFiles);
			else
				MoveFilesToOutputDir(textureFiles);
		}

		// Embed vtf/vmt files into BSP pak lump
		private void EmbedFiles(IEnumerable<string> textureFiles)
		{
			// TODO: Only create a zip archive if one doesn't exist
			using (var archive = ZipArchive.Create())
			{
				foreach (var file in textureFiles)
				{
					var newPath = file.Replace(pk3Dir, "materials");
					archive.AddEntry(newPath, new FileInfo(file));
				}

				bsp.PakFile.SetZipArchive(archive, true);
			}
		}

		// Move vtf/vmt files into output directory
		private void MoveFilesToOutputDir(IEnumerable<string> textureFiles)
		{
			foreach (var file in textureFiles)
			{
				var materialDir = Path.Combine(outputDir, "materials");
				var newPath = file.Replace(pk3Dir, materialDir);
				FileUtil.MoveFile(file, newPath);
			}
		}
	}
}
