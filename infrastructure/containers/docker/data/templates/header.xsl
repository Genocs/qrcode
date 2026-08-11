<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:utilityExtension="pdfprinter:extensions:utility"
	xmlns:fo="http://www.w3.org/1999/XSL/Format"
	xmlns:svg="http://www.w3.org/2000/svg"
	xmlns:xlink="http://www.w3.org/1999/xlink">
	<xsl:variable name="logo" select="utilityExtension:MapPath('./data/templates/browserstack-logo.svg')"/>
	
	<xsl:template name="header-first-page-template">
		<fo:static-content flow-name="header-first-region">
			<fo:block font-size="6.5pt">
				<fo:table>
					<fo:table-column column-width="130mm"/>
					<fo:table-column column-width="50mm"/>
					<fo:table-body>
						<fo:table-row>
							<fo:table-cell>
								<fo:block font-size="8pt" text-align="left">
									<fo:block margin-bottom="0.1cm" font-size="14pt" font-weight="bold">
										Genocs
									</fo:block>
									<fo:block margin-top="0.1cm" margin-bottom="0.1cm">
										Software Technology and many more
									</fo:block>
									<fo:block margin-top="0.1cm" margin-bottom="0.1cm">
										Tech company specialized in software development and IT consulting
									</fo:block>
									<fo:block margin-top="0.1cm" margin-bottom="0.1cm">
										Via Trasimeno 40/10 - 20128 Milano (MI)
									</fo:block>
									<fo:block margin-top="0.2cm" margin-bottom="0.1cm">
										P.IVA: 03518950757
									</fo:block>
									<fo:block margin-top="0.1cm" margin-bottom="0.1cm">
										<fo:basic-link external-destination="https://www.genocs.com" color="blue">https://www.genocs.com</fo:basic-link>
									</fo:block>
								</fo:block>
							</fo:table-cell>
							<fo:table-cell text-align="left">
								<fo:instream-foreign-object>
									<svg:svg width="90mm" height="20mm" viewBox="0 0 90 20">
										<svg:path id="icon" fill="#0000A0" fill-opacity="0.2" d="M70.007,121.541c0-28.41,23.114-51.521,51.526-51.521c1.787,0,3.552,0.088,5.293,0.272
												c29.685,2.7,41.925,27.425,61.944,27.425c20.005,0,32.252-24.725,61.934-27.425c1.742-0.185,3.503-0.272,5.29-0.272
												c28.41,0,51.531,23.111,51.531,51.521c0,28.406-23.121,51.522-51.531,51.522l0,0c-33.61,0-46.063-27.696-67.224-27.696
												c-21.174,0-33.621,27.696-67.237,27.696l0,0C93.122,173.063,70.007,149.947,70.007,121.541z M255.994,204.48
												c-28.408,0-51.516,23.109-51.516,51.521c0,28.401,23.108,51.517,51.516,51.517c28.41,0,51.531-23.115,51.531-51.517
												C307.525,227.59,284.404,204.48,255.994,204.48z M390.463,173.063c28.408,0,51.529-23.116,51.529-51.522
												c0-28.41-23.121-51.521-51.529-51.521c-28.414,0-51.521,23.111-51.521,51.521C338.941,149.947,362.049,173.063,390.463,173.063z
												M390.463,338.934L390.463,338.934c-0.918,0-1.765,0.101-2.65,0.137c-0.879,0.042-1.758,0.042-2.629,0.134
												c-29.691,2.695-41.938,27.428-61.954,27.428c-20.021,0-32.269-24.732-61.953-27.428c-0.866-0.092-1.752-0.092-2.631-0.134
												c-0.885-0.036-1.726-0.137-2.651-0.137l0,0c-1.787,0-3.548,0.095-5.29,0.271c-29.681,2.705-41.929,27.428-61.934,27.428
												c-10.714,0-19.202-7.091-29.316-14.094c-7.009-10.114-14.1-18.599-14.1-29.307c0-20.015,24.743-32.266,27.432-61.96
												c0.091-0.85,0.091-1.741,0.14-2.61c0.039-0.889,0.133-1.738,0.133-2.66h-0.003c0-28.411-23.114-51.521-51.522-51.521
												c-28.412,0-51.526,23.109-51.526,51.521l0,0c0,0.922,0.094,1.771,0.14,2.66c0.042,0.869,0.049,1.761,0.13,2.61
												c2.696,29.694,27.428,41.945,27.428,61.96c0,20.016-24.732,32.26-27.428,61.946c-0.088,0.867-0.088,1.762-0.137,2.645
												c-0.039,0.882-0.133,1.722-0.133,2.633l0,0c0,28.398,23.114,51.526,51.526,51.526l0,0c33.617,0,46.063-27.702,67.237-27.702
												c21.161,0,33.613,27.702,67.224,27.702l0,0c0.926,0,1.767-0.102,2.651-0.141c0.879-0.039,1.765-0.039,2.631-0.127
												c29.685-2.698,41.932-27.438,61.953-27.438c20.016,0,32.263,24.739,61.954,27.438c0.871,0.088,1.75,0.088,2.629,0.127
												c0.886,0.039,1.732,0.141,2.65,0.141l0,0c28.408,0,51.529-23.128,51.529-51.526C441.992,362.045,418.871,338.934,390.463,338.934z
												M390.463,204.48c-28.414,0-51.521,23.109-51.521,51.521c0,28.401,23.107,51.517,51.521,51.517
												c28.408,0,51.529-23.115,51.529-51.517C441.992,227.59,418.871,204.48,390.463,204.48z"/>
									</svg:svg>
								</fo:instream-foreign-object>
							</fo:table-cell>
						</fo:table-row>
					</fo:table-body>
				</fo:table>
			</fo:block>
		</fo:static-content>
	</xsl:template>
	
	<xsl:template name="header-other-pages-template">
		<fo:static-content flow-name="header-other-region">
			<fo:block font-size="8pt" space-after="2pt">
				<fo:table table-layout="fixed" width="100%">
					<fo:table-column column-width="proportional-column-width(1)"/>
					<fo:table-column column-width="18mm"/>
					<fo:table-body>
						<fo:table-row>
							<fo:table-cell display-align="center">
								<fo:block font-size="9pt" font-weight="bold">
									Genocs Software Technology
								</fo:block>
								<fo:block font-size="7pt" color="rgb(80, 80, 80)">
									Books Report
								</fo:block>
							</fo:table-cell>
							<fo:table-cell display-align="center" text-align="right">
								<fo:external-graphic src="url('{$logo}')" content-height="10mm" scaling="uniform" scaling-method="resample-any-method"/>
							</fo:table-cell>
						</fo:table-row>
					</fo:table-body>
				</fo:table>
			</fo:block>
			<fo:block>
				<fo:leader leader-pattern="rule" rule-thickness="0.5pt" leader-length="100%" color="rgb(128,128,128)"/>
			</fo:block>
		</fo:static-content>
	</xsl:template>
</xsl:stylesheet>
