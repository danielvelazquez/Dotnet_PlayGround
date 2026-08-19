using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Playwrite.LectorDePagina.Models;

using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playwrite.LectorDePagina.Servicios
{
    public class PageReaderService : IPageReaderService
    {

        public Task CalculateSpikes(TvTable tvTable)
        {
            throw new NotImplementedException();
        }

        public Task LoadTransviewerReport(string pageUrl)
        {
            throw new NotImplementedException();
        }

        public Task ReadTransviewerTable(TvTable tvTable)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, string>> ReadBanxicoIndicators()
        {
            var indicators = new Dictionary<string, string>();

            // Inicializar Playwright
            using var playwright = await Playwright.CreateAsync();
            
            // Lanzar navegador (puedes usar Chromium, Firefox o WebKit)
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true // Cambia a false si quieres ver el navegador
            });

            // Crear contexto y página
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            try
            {
                // Navegar a la página de Banxico
                await page.GotoAsync("https://www.banxico.org.mx/", new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = 30000
                });

                // Esperar a que el div con clase "indicadores" esté visible
                await page.WaitForSelectorAsync("div.indicadores", new PageWaitForSelectorOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 10000
                });

                // Localizar el div de indicadores
                var indicadoresDiv = page.Locator("div.indicadores");

                // Obtener todos los elementos dentro del div
                var items = indicadoresDiv.Locator(".renglonNombre, .renglonValor");
                var count = await items.CountAsync();

                string? currentName = null;

                // Iterar sobre los elementos
                for (int i = 0; i < count; i++)
                {
                    var item = items.Nth(i);
                    var className = await item.GetAttributeAsync("class");
                    var text = await item.TextContentAsync();

                    if (className?.Contains("renglonNombre") == true)
                    {
                        currentName = text?.Trim();
                    }
                    else if (className?.Contains("renglonValor") == true && !string.IsNullOrEmpty(currentName))
                    {
                        indicators[currentName] = text?.Trim() ?? string.Empty;
                        currentName = null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log del error (considera usar ILogger aquí)
                Console.WriteLine($"Error al leer indicadores de Banxico: {ex.Message}");
                throw;
            }
            finally
            {
                await page.CloseAsync();
                await context.CloseAsync();
            }

            return indicators;
        }

        public async Task WhenLoadingPage_SelectDatabase()
        {
            await Page.GotoAsync("https://transviewer.ual.com/transviewer/Report.aspx", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var databaseSelector = Page.Locator("cboDatabase");
            await databaseSelector.ClickAsync();
            var selectResult = await Page.WaitForSelectorAsync("cboDatabase", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            await selectResult.SelectOptionAsync("eKiosk Production - AWS");


        }

        [TestMethod]
        public async Task WhenLoadingPage_ShowTableResult()
        {
            await Page.GotoAsync("https://transviewer.ual.com/transviewer/Report.aspx", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var databaseSelector = Page.Locator("select#cboDatabase");

            await databaseSelector.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            await databaseSelector.SelectOptionAsync(new SelectOptionValue { Value = "eKiosk Production - AWS" });

            var actionSelector = Page.Locator("select#cboSearch");
            await actionSelector.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            await actionSelector.SelectOptionAsync(new SelectOptionValue { Value = "104" });


            await Page.WaitForTimeoutAsync(500);

            var submitButton = Page.Locator("input[id$='btnSubmit']");

            await submitButton.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            await submitButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);


            await Page.WaitForTimeoutAsync(1000);


            var tableResult = Page.Locator("table#tblResult");


            Assert.IsTrue(tableResult != null);
        }

//        <table id = "tblResult" cellspacing="1" cellpadding="6" style="background-color:LightGrey;font-family:Arial;font-size:9pt;visibility:visible;table-layout:fixed;width:94%px;">
//	<tbody><tr>
//		<td class="tblHeader" align="center" style="width:30px;white-space:nowrap;">&nbsp;</td><td class="tblHeader" align="center" style="width:70px;white-space:nowrap;">Error<br> Code</td><td class="tblHeader" align="center" style="width:300px;white-space:nowrap;">Error Description</td><td class="tblHeader" align="center" style="width:75px;white-space:nowrap;">03 Mar</td><td class="tblHeader" align="center" style="width:75px;white-space:nowrap;">02 Mar</td><td class="tblHeader" align="center" style="width:75px;white-space:nowrap;">01 Mar</td>
//	</tr><tr id = "row0" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">1</td><td align = "left" valign="top" onclick="selectRow(0);">50203</td><td align = "left" valign="top" onclick="selectRow(0);">DB_LATENCY</td><td align = "right" valign="top" onclick="selectRow(0);">4,707</td><td align = "right" valign="top" onclick="selectRow(0);">9,996</td><td align = "right" valign="top" onclick="selectRow(0);">14,632</td>
//	</tr><tr id = "row1" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">2</td><td align = "left" valign="top" onclick="selectRow(1);">50022</td><td align = "left" valign="top" onclick="selectRow(1);">SELFTAG_R1_ACTIVATE_ERROR</td><td align = "right" valign="top" onclick="selectRow(1);">2,853</td><td align = "right" valign="top" onclick="selectRow(1);">4,411</td><td align = "right" valign="top" onclick="selectRow(1);">5,175</td>
//	</tr><tr id = "row2" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">3</td><td align = "left" valign="top" onclick="selectRow(2);">50015</td><td align = "left" valign="top" onclick="selectRow(2);">PNR_NOT_FOUND</td><td align = "right" valign="top" onclick="selectRow(2);">2,180</td><td align = "right" valign="top" onclick="selectRow(2);">3,762</td><td align = "right" valign="top" onclick="selectRow(2);">3,973</td>
//	</tr><tr id = "row3" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">4</td><td align = "left" valign="top" onclick="selectRow(3);">70009</td><td align = "left" valign="top" onclick="selectRow(3);">API_PNR_NOT_FOUND</td><td align = "right" valign="top" onclick="selectRow(3);">1,743</td><td align = "right" valign="top" onclick="selectRow(3);">3,112</td><td align = "right" valign="top" onclick="selectRow(3);">3,378</td>
//	</tr><tr id = "row4" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">5</td><td align = "left" valign="top" onclick="selectRow(4);">50195</td><td align = "left" valign="top" onclick="selectRow(4);">MISSING_CAPTION</td><td align = "right" valign="top" onclick="selectRow(4);">1,724</td><td align = "right" valign="top" onclick="selectRow(4);">3,056</td><td align = "right" valign="top" onclick="selectRow(4);">3,236</td>
//	</tr><tr id = "row5" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">6</td><td align = "left" valign="top" onclick="selectRow(5);">50047</td><td align = "left" valign="top" onclick="selectRow(5);">SHARES_LATENCY</td><td align = "right" valign="top" onclick="selectRow(5);">1,446</td><td align = "right" valign="top" onclick="selectRow(5);">2,098</td><td align = "right" valign="top" onclick="selectRow(5);">1,748</td>
//	</tr><tr id = "row6" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">7</td><td align = "left" valign="top" onclick="selectRow(6);">50140</td><td align = "left" valign="top" onclick="selectRow(6);">PASSPORT_INVALID_NAME_MATCH</td><td align = "right" valign="top" onclick="selectRow(6);">892</td><td align = "right" valign="top" onclick="selectRow(6);">1,279</td><td align = "right" valign="top" onclick="selectRow(6);">1,504</td>
//	</tr><tr id = "row7" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">8</td><td align = "left" valign="top" onclick="selectRow(7);">50334</td><td align = "left" valign="top" onclick="selectRow(7);">BAG_POLICY_ERROR</td><td align = "right" valign="top" onclick="selectRow(7);">846</td><td align = "right" valign="top" onclick="selectRow(7);">1,322</td><td align = "right" valign="top" onclick="selectRow(7);">1,484</td>
//	</tr><tr id = "row8" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">9</td><td align = "left" valign="top" onclick="selectRow(8);">50135</td><td align = "left" valign="top" onclick="selectRow(8);">INTL_NO_TIMATIC_ENTRY</td><td align = "right" valign="top" onclick="selectRow(8);">826</td><td align = "right" valign="top" onclick="selectRow(8);">1,297</td><td align = "right" valign="top" onclick="selectRow(8);">1,483</td>
//	</tr><tr id = "row9" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">10</td><td align = "left" valign="top" onclick="selectRow(9);">50063</td><td align = "left" valign="top" onclick="selectRow(9);">NETWORK_LATENCY</td><td align = "right" valign="top" onclick="selectRow(9);">740</td><td align = "right" valign="top" onclick="selectRow(9);">1,867</td><td align = "right" valign="top" onclick="selectRow(9);">2,543</td>
//	</tr><tr id = "row10" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">11</td><td align = "left" valign="top" onclick="selectRow(10);">50327</td><td align = "left" valign="top" onclick="selectRow(10);">FLIGHT_RANGE_MISSING_DEI</td><td align = "right" valign="top" onclick="selectRow(10);">483</td><td align = "right" valign="top" onclick="selectRow(10);">676</td><td align = "right" valign="top" onclick="selectRow(10);">623</td>
//	</tr><tr id = "row11" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">12</td><td align = "left" valign="top" onclick="selectRow(11);">50018</td><td align = "left" valign="top" onclick="selectRow(11);">PNR_GROUP_FOUND</td><td align = "right" valign="top" onclick="selectRow(11);">482</td><td align = "right" valign="top" onclick="selectRow(11);">864</td><td align = "right" valign="top" onclick="selectRow(11);">1,195</td>
//	</tr><tr id = "row12" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">13</td><td align = "left" valign="top" onclick="selectRow(12);">50272</td><td align = "left" valign="top" onclick="selectRow(12);">MISSING_TRANSLATION</td><td align = "right" valign="top" onclick="selectRow(12);">463</td><td align = "right" valign="top" onclick="selectRow(12);">750</td><td align = "right" valign="top" onclick="selectRow(12);">895</td>
//	</tr><tr id = "row13" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">14</td><td align = "left" valign="top" onclick="selectRow(13);">50263</td><td align = "left" valign="top" onclick="selectRow(13);">NOBAG_CUTOFF_TIME</td><td align = "right" valign="top" onclick="selectRow(13);">456</td><td align = "right" valign="top" onclick="selectRow(13);">900</td><td align = "right" valign="top" onclick="selectRow(13);">844</td>
//	</tr><tr id = "row14" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">15</td><td align = "left" valign="top" onclick="selectRow(14);">50399</td><td align = "left" valign="top" onclick="selectRow(14);">MERCH_OFFER_FAILED</td><td align = "right" valign="top" onclick="selectRow(14);">434</td><td align = "right" valign="top" onclick="selectRow(14);">110</td><td align = "right" valign="top" onclick="selectRow(14);">145</td>
//	</tr><tr id = "row15" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">16</td><td align = "left" valign="top" onclick="selectRow(15);">50181</td><td align = "left" valign="top" onclick="selectRow(15);">PASSPORT_PREVIOUS_SCANNED</td><td align = "right" valign="top" onclick="selectRow(15);">419</td><td align = "right" valign="top" onclick="selectRow(15);">602</td><td align = "right" valign="top" onclick="selectRow(15);">735</td>
//	</tr><tr id = "row16" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">17</td><td align = "left" valign="top" onclick="selectRow(16);">50142</td><td align = "left" valign="top" onclick="selectRow(16);">PASSPORT_BAD_DOCUMENT_READ</td><td align = "right" valign="top" onclick="selectRow(16);">391</td><td align = "right" valign="top" onclick="selectRow(16);">691</td><td align = "right" valign="top" onclick="selectRow(16);">809</td>
//	</tr><tr id = "row17" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">18</td><td align = "left" valign="top" onclick="selectRow(17);">50268</td><td align = "left" valign="top" onclick="selectRow(17);">DOD_ERROR</td><td align = "right" valign="top" onclick="selectRow(17);">265</td><td align = "right" valign="top" onclick="selectRow(17);">412</td><td align = "right" valign="top" onclick="selectRow(17);">498</td>
//	</tr><tr id = "row18" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">19</td><td align = "left" valign="top" onclick="selectRow(18);">50028</td><td align = "left" valign="top" onclick="selectRow(18);">PNR_FLIGHT_TOO_LATE_FOR_CHECKIN</td><td align = "right" valign="top" onclick="selectRow(18);">250</td><td align = "right" valign="top" onclick="selectRow(18);">513</td><td align = "right" valign="top" onclick="selectRow(18);">522</td>
//	</tr><tr id = "row19" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">20</td><td align = "left" valign="top" onclick="selectRow(19);">50037</td><td align = "left" valign="top" onclick="selectRow(19);">CUSTOMER_SELECTEE</td><td align = "right" valign="top" onclick="selectRow(19);">239</td><td align = "right" valign="top" onclick="selectRow(19);">404</td><td align = "right" valign="top" onclick="selectRow(19);">444</td>
//	</tr><tr id = "row20" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">21</td><td align = "left" valign="top" onclick="selectRow(20);">50147</td><td align = "left" valign="top" onclick="selectRow(20);">NOFLY_ERROR</td><td align = "right" valign="top" onclick="selectRow(20);">239</td><td align = "right" valign="top" onclick="selectRow(20);">430</td><td align = "right" valign="top" onclick="selectRow(20);">516</td>
//	</tr><tr id = "row21" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">22</td><td align = "left" valign="top" onclick="selectRow(21);">50056</td><td align = "left" valign="top" onclick="selectRow(21);">RESYNC_NO_COUPON_MATCH</td><td align = "right" valign="top" onclick="selectRow(21);">214</td><td align = "right" valign="top" onclick="selectRow(21);">389</td><td align = "right" valign="top" onclick="selectRow(21);">249</td>
//	</tr><tr id = "row22" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">23</td><td align = "left" valign="top" onclick="selectRow(22);">50155</td><td align = "left" valign="top" onclick="selectRow(22);">PRC_SCAN_ERROR</td><td align = "right" valign="top" onclick="selectRow(22);">196</td><td align = "right" valign="top" onclick="selectRow(22);">346</td><td align = "right" valign="top" onclick="selectRow(22);">370</td>
//	</tr><tr id = "row23" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">24</td><td align = "left" valign="top" onclick="selectRow(23);">70020</td><td align = "left" valign="top" onclick="selectRow(23);">API_AVAILABILITY_ERROR</td><td align = "right" valign="top" onclick="selectRow(23);">174</td><td align = "right" valign="top" onclick="selectRow(23);">265</td><td align = "right" valign="top" onclick="selectRow(23);">311</td>
//	</tr><tr id = "row24" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">25</td><td align = "left" valign="top" onclick="selectRow(24);">50143</td><td align = "left" valign="top" onclick="selectRow(24);">CARD_INVALID_DATA</td><td align = "right" valign="top" onclick="selectRow(24);">174</td><td align = "right" valign="top" onclick="selectRow(24);">312</td><td align = "right" valign="top" onclick="selectRow(24);">314</td>
//	</tr><tr id = "row25" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">26</td><td align = "left" valign="top" onclick="selectRow(25);">50364</td><td align = "left" valign="top" onclick="selectRow(25);">BCC_BAD_DOCUMENT_READ</td><td align = "right" valign="top" onclick="selectRow(25);">170</td><td align = "right" valign="top" onclick="selectRow(25);">291</td><td align = "right" valign="top" onclick="selectRow(25);">304</td>
//	</tr><tr id = "row26" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">27</td><td align = "left" valign="top" onclick="selectRow(26);">50191</td><td align = "left" valign="top" onclick="selectRow(26);">SEATMAP_LOAD_FAILED</td><td align = "right" valign="top" onclick="selectRow(26);">151</td><td align = "right" valign="top" onclick="selectRow(26);">248</td><td align = "right" valign="top" onclick="selectRow(26);">267</td>
//	</tr><tr id = "row27" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">28</td><td align = "left" valign="top" onclick="selectRow(27);">50424</td><td align = "left" valign="top" onclick="selectRow(27);">BAG_ACK_ERROR</td><td align = "right" valign="top" onclick="selectRow(27);">141</td><td align = "right" valign="top" onclick="selectRow(27);">332</td><td align = "right" valign="top" onclick="selectRow(27);">311</td>
//	</tr><tr id = "row28" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">29</td><td align = "left" valign="top" onclick="selectRow(28);">50431</td><td align = "left" valign="top" onclick="selectRow(28);">RESHOP_REPRICE_FAILED</td><td align = "right" valign="top" onclick="selectRow(28);">139</td><td align = "right" valign="top" onclick="selectRow(28);">234</td><td align = "right" valign="top" onclick="selectRow(28);">204</td>
//	</tr><tr id = "row29" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">30</td><td align = "left" valign="top" onclick="selectRow(29);">50029</td><td align = "left" valign="top" onclick="selectRow(29);">PNR_NOT_ETICKETED</td><td align = "right" valign="top" onclick="selectRow(29);">138</td><td align = "right" valign="top" onclick="selectRow(29);">319</td><td align = "right" valign="top" onclick="selectRow(29);">301</td>
//	</tr><tr id = "row30" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">31</td><td align = "left" valign="top" onclick="selectRow(30);">50274</td><td align = "left" valign="top" onclick="selectRow(30);">PNR_FIRST_SEGMENT_OA_STAR</td><td align = "right" valign="top" onclick="selectRow(30);">134</td><td align = "right" valign="top" onclick="selectRow(30);">248</td><td align = "right" valign="top" onclick="selectRow(30);">264</td>
//	</tr><tr id = "row31" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">32</td><td align = "left" valign="top" onclick="selectRow(31);">50021</td><td align = "left" valign="top" onclick="selectRow(31);">FLIGHT_RANGE_ERROR</td><td align = "right" valign="top" onclick="selectRow(31);">131</td><td align = "right" valign="top" onclick="selectRow(31);">270</td><td align = "right" valign="top" onclick="selectRow(31);">166</td>
//	</tr><tr id = "row32" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">33</td><td align = "left" valign="top" onclick="selectRow(32);">50038</td><td align = "left" valign="top" onclick="selectRow(32);">EMPLOYEE_JA_NOT_RETRIEVED</td><td align = "right" valign="top" onclick="selectRow(32);">122</td><td align = "right" valign="top" onclick="selectRow(32);">175</td><td align = "right" valign="top" onclick="selectRow(32);">265</td>
//	</tr><tr id = "row33" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">34</td><td align = "left" valign="top" onclick="selectRow(33);">50271</td><td align = "left" valign="top" onclick="selectRow(33);">PNR_SEGMENTS_USED</td><td align = "right" valign="top" onclick="selectRow(33);">121</td><td align = "right" valign="top" onclick="selectRow(33);">214</td><td align = "right" valign="top" onclick="selectRow(33);">200</td>
//	</tr><tr id = "row34" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">35</td><td align = "left" valign="top" onclick="selectRow(34);">50027</td><td align = "left" valign="top" onclick="selectRow(34);">PNR_FLIGHT_TOO_EARLY_FOR_CHECKIN</td><td align = "right" valign="top" onclick="selectRow(34);">114</td><td align = "right" valign="top" onclick="selectRow(34);">182</td><td align = "right" valign="top" onclick="selectRow(34);">201</td>
//	</tr><tr id = "row35" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">36</td><td align = "left" valign="top" onclick="selectRow(35);">50405</td><td align = "left" valign="top" onclick="selectRow(35);">UCD_SERVICE_ERROR</td><td align = "right" valign="top" onclick="selectRow(35);">102</td><td align = "right" valign="top" onclick="selectRow(35);">141</td><td align = "right" valign="top" onclick="selectRow(35);">169</td>
//	</tr><tr id = "row36" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">37</td><td align = "left" valign="top" onclick="selectRow(36);">50013</td><td align = "left" valign="top" onclick="selectRow(36);">SEARCH_TOO_MANY_PNR_FOUND</td><td align = "right" valign="top" onclick="selectRow(36);">99</td><td align = "right" valign="top" onclick="selectRow(36);">146</td><td align = "right" valign="top" onclick="selectRow(36);">199</td>
//	</tr><tr id = "row37" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">38</td><td align = "left" valign="top" onclick="selectRow(37);">50084</td><td align = "left" valign="top" onclick="selectRow(37);">CHECKIN_FAILED</td><td align = "right" valign="top" onclick="selectRow(37);">94</td><td align = "right" valign="top" onclick="selectRow(37);">110</td><td align = "right" valign="top" onclick="selectRow(37);">190</td>
//	</tr><tr id = "row38" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">39</td><td align = "left" valign="top" onclick="selectRow(38);">50265</td><td align = "left" valign="top" onclick="selectRow(38);">FLIGHT_CARRIER_MISSING</td><td align = "right" valign="top" onclick="selectRow(38);">91</td><td align = "right" valign="top" onclick="selectRow(38);">180</td><td align = "right" valign="top" onclick="selectRow(38);">187</td>
//	</tr><tr id = "row39" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">40</td><td align = "left" valign="top" onclick="selectRow(39);">50020</td><td align = "left" valign="top" onclick="selectRow(39);">PNR_SEGMENTS_NOT_FOUND</td><td align = "right" valign="top" onclick="selectRow(39);">84</td><td align = "right" valign="top" onclick="selectRow(39);">123</td><td align = "right" valign="top" onclick="selectRow(39);">153</td>
//	</tr><tr id = "row40" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">41</td><td align = "left" valign="top" onclick="selectRow(40);">50444</td><td align = "left" valign="top" onclick="selectRow(40);">PREDICTEDBAG_SERVICE_ERROR</td><td align = "right" valign="top" onclick="selectRow(40);">83</td><td align = "right" valign="top" onclick="selectRow(40);">125</td><td align = "right" valign="top" onclick="selectRow(40);">142</td>
//	</tr><tr id = "row41" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">42</td><td align = "left" valign="top" onclick="selectRow(41);">50124</td><td align = "left" valign="top" onclick="selectRow(41);">CARD_AUTHORIZATION_FAILED</td><td align = "right" valign="top" onclick="selectRow(41);">78</td><td align = "right" valign="top" onclick="selectRow(41);">149</td><td align = "right" valign="top" onclick="selectRow(41);">162</td>
//	</tr><tr id = "row42" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">43</td><td align = "left" valign="top" onclick="selectRow(42);">50113</td><td align = "left" valign="top" onclick="selectRow(42);">EBP_ERROR</td><td align = "right" valign="top" onclick="selectRow(42);">74</td><td align = "right" valign="top" onclick="selectRow(42);">118</td><td align = "right" valign="top" onclick="selectRow(42);">94</td>
//	</tr><tr id = "row43" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">44</td><td align = "left" valign="top" onclick="selectRow(43);">50256</td><td align = "left" valign="top" onclick="selectRow(43);">DELETED_BAGTAG_INVALID</td><td align = "right" valign="top" onclick="selectRow(43);">72</td><td align = "right" valign="top" onclick="selectRow(43);">123</td><td align = "right" valign="top" onclick="selectRow(43);">117</td>
//	</tr><tr id = "row44" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">45</td><td align = "left" valign="top" onclick="selectRow(44);">50115</td><td align = "left" valign="top" onclick="selectRow(44);">DOC_EXPIRED</td><td align = "right" valign="top" onclick="selectRow(44);">65</td><td align = "right" valign="top" onclick="selectRow(44);">86</td><td align = "right" valign="top" onclick="selectRow(44);">119</td>
//	</tr><tr id = "row45" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">46</td><td align = "left" valign="top" onclick="selectRow(45);">50375</td><td align = "left" valign="top" onclick="selectRow(45);">KIOSK_DIFFERENT_LOCATION</td><td align = "right" valign="top" onclick="selectRow(45);">64</td><td align = "right" valign="top" onclick="selectRow(45);">90</td><td align = "right" valign="top" onclick="selectRow(45);">91</td>
//	</tr><tr id = "row46" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">47</td><td align = "left" valign="top" onclick="selectRow(46);">50350</td><td align = "left" valign="top" onclick="selectRow(46);">TCD_RETRIEVE_ERROR</td><td align = "right" valign="top" onclick="selectRow(46);">60</td><td align = "right" valign="top" onclick="selectRow(46);">102</td><td align = "right" valign="top" onclick="selectRow(46);">95</td>
//	</tr><tr id = "row47" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">48</td><td align = "left" valign="top" onclick="selectRow(47);">50000</td><td align = "left" valign="top" onclick="selectRow(47);">DB_DATABASE_ERROR</td><td align = "right" valign="top" onclick="selectRow(47);">54</td><td align = "right" valign="top" onclick="selectRow(47);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(47);">&nbsp;</td>
//	</tr><tr id = "row48" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">49</td><td align = "left" valign="top" onclick="selectRow(48);">50150</td><td align = "left" valign="top" onclick="selectRow(48);">SSR_PET</td><td align = "right" valign="top" onclick="selectRow(48);">51</td><td align = "right" valign="top" onclick="selectRow(48);">49</td><td align = "right" valign="top" onclick="selectRow(48);">51</td>
//	</tr><tr id = "row49" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">50</td><td align = "left" valign="top" onclick="selectRow(49);">49998</td><td align = "left" valign="top" onclick="selectRow(49);">UNKNOWN_ERROR</td><td align = "right" valign="top" onclick="selectRow(49);">50</td><td align = "right" valign="top" onclick="selectRow(49);">65</td><td align = "right" valign="top" onclick="selectRow(49);">78</td>
//	</tr><tr id = "row50" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">51</td><td align = "left" valign="top" onclick="selectRow(50);">50316</td><td align = "left" valign="top" onclick="selectRow(50);">PROFILE_NAME_MISMATCH</td><td align = "right" valign="top" onclick="selectRow(50);">45</td><td align = "right" valign="top" onclick="selectRow(50);">56</td><td align = "right" valign="top" onclick="selectRow(50);">67</td>
//	</tr><tr id = "row51" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">52</td><td align = "left" valign="top" onclick="selectRow(51);">50320</td><td align = "left" valign="top" onclick="selectRow(51);">SELFTAG_PRINT_ERROR</td><td align = "right" valign="top" onclick="selectRow(51);">43</td><td align = "right" valign="top" onclick="selectRow(51);">104</td><td align = "right" valign="top" onclick="selectRow(51);">124</td>
//	</tr><tr id = "row52" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">53</td><td align = "left" valign="top" onclick="selectRow(52);">50095</td><td align = "left" valign="top" onclick="selectRow(52);">BAGS_ADD_FAILED</td><td align = "right" valign="top" onclick="selectRow(52);">42</td><td align = "right" valign="top" onclick="selectRow(52);">65</td><td align = "right" valign="top" onclick="selectRow(52);">47</td>
//	</tr><tr id = "row53" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">54</td><td align = "left" valign="top" onclick="selectRow(53);">50381</td><td align = "left" valign="top" onclick="selectRow(53);">SHARES_API_ERROR</td><td align = "right" valign="top" onclick="selectRow(53);">41</td><td align = "right" valign="top" onclick="selectRow(53);">46</td><td align = "right" valign="top" onclick="selectRow(53);">18</td>
//	</tr><tr id = "row54" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">55</td><td align = "left" valign="top" onclick="selectRow(54);">50078</td><td align = "left" valign="top" onclick="selectRow(54);">CHANGE_SEAT_FAILED</td><td align = "right" valign="top" onclick="selectRow(54);">40</td><td align = "right" valign="top" onclick="selectRow(54);">76</td><td align = "right" valign="top" onclick="selectRow(54);">55</td>
//	</tr><tr id = "row55" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">56</td><td align = "left" valign="top" onclick="selectRow(55);">50194</td><td align = "left" valign="top" onclick="selectRow(55);">PASSPORT_INVALID_NUMBER</td><td align = "right" valign="top" onclick="selectRow(55);">38</td><td align = "right" valign="top" onclick="selectRow(55);">43</td><td align = "right" valign="top" onclick="selectRow(55);">53</td>
//	</tr><tr id = "row56" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">57</td><td align = "left" valign="top" onclick="selectRow(56);">50196</td><td align = "left" valign="top" onclick="selectRow(56);">MISSING_AIRPORT</td><td align = "right" valign="top" onclick="selectRow(56);">38</td><td align = "right" valign="top" onclick="selectRow(56);">141</td><td align = "right" valign="top" onclick="selectRow(56);">84</td>
//	</tr><tr id = "row57" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">58</td><td align = "left" valign="top" onclick="selectRow(57);">50345</td><td align = "left" valign="top" onclick="selectRow(57);">INVALID_REQUEST</td><td align = "right" valign="top" onclick="selectRow(57);">35</td><td align = "right" valign="top" onclick="selectRow(57);">55</td><td align = "right" valign="top" onclick="selectRow(57);">40</td>
//	</tr><tr id = "row58" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">59</td><td align = "left" valign="top" onclick="selectRow(58);">50235</td><td align = "left" valign="top" onclick="selectRow(58);">PAYMENT_REQUIRED</td><td align = "right" valign="top" onclick="selectRow(58);">28</td><td align = "right" valign="top" onclick="selectRow(58);">23</td><td align = "right" valign="top" onclick="selectRow(58);">31</td>
//	</tr><tr id = "row59" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">60</td><td align = "left" valign="top" onclick="selectRow(59);">50026</td><td align = "left" valign="top" onclick="selectRow(59);">PNR_RESTRICTED_TO_GATE</td><td align = "right" valign="top" onclick="selectRow(59);">27</td><td align = "right" valign="top" onclick="selectRow(59);">48</td><td align = "right" valign="top" onclick="selectRow(59);">42</td>
//	</tr><tr id = "row60" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">61</td><td align = "left" valign="top" onclick="selectRow(60);">50076</td><td align = "left" valign="top" onclick="selectRow(60);">FQTV_NOT_UPDATED</td><td align = "right" valign="top" onclick="selectRow(60);">27</td><td align = "right" valign="top" onclick="selectRow(60);">34</td><td align = "right" valign="top" onclick="selectRow(60);">35</td>
//	</tr><tr id = "row61" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">62</td><td align = "left" valign="top" onclick="selectRow(61);">50137</td><td align = "left" valign="top" onclick="selectRow(61);">INTL_APIS_SEND_ERROR</td><td align = "right" valign="top" onclick="selectRow(61);">27</td><td align = "right" valign="top" onclick="selectRow(61);">49</td><td align = "right" valign="top" onclick="selectRow(61);">56</td>
//	</tr><tr id = "row62" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">63</td><td align = "left" valign="top" onclick="selectRow(62);">70069</td><td align = "left" valign="top" onclick="selectRow(62);">API_FQTV_NOT_UPDATED</td><td align = "right" valign="top" onclick="selectRow(62);">26</td><td align = "right" valign="top" onclick="selectRow(62);">33</td><td align = "right" valign="top" onclick="selectRow(62);">35</td>
//	</tr><tr id = "row63" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">64</td><td align = "left" valign="top" onclick="selectRow(63);">50367</td><td align = "left" valign="top" onclick="selectRow(63);">INVALID_NAME_MATCH</td><td align = "right" valign="top" onclick="selectRow(63);">25</td><td align = "right" valign="top" onclick="selectRow(63);">51</td><td align = "right" valign="top" onclick="selectRow(63);">66</td>
//	</tr><tr id = "row64" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">65</td><td align = "left" valign="top" onclick="selectRow(64);">50322</td><td align = "left" valign="top" onclick="selectRow(64);">SELFTAG_R1_UPDATE_ERROR</td><td align = "right" valign="top" onclick="selectRow(64);">23</td><td align = "right" valign="top" onclick="selectRow(64);">54</td><td align = "right" valign="top" onclick="selectRow(64);">41</td>
//	</tr><tr id = "row65" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">66</td><td align = "left" valign="top" onclick="selectRow(65);">50457</td><td align = "left" valign="top" onclick="selectRow(65);">SELFTAG_R2_DELETE_ERROR</td><td align = "right" valign="top" onclick="selectRow(65);">22</td><td align = "right" valign="top" onclick="selectRow(65);">55</td><td align = "right" valign="top" onclick="selectRow(65);">41</td>
//	</tr><tr id = "row66" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">67</td><td align = "left" valign="top" onclick="selectRow(66);">50030</td><td align = "left" valign="top" onclick="selectRow(66);">SELFTAG_R1_NUMBER_ERROR</td><td align = "right" valign="top" onclick="selectRow(66);">21</td><td align = "right" valign="top" onclick="selectRow(66);">53</td><td align = "right" valign="top" onclick="selectRow(66);">35</td>
//	</tr><tr id = "row67" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">68</td><td align = "left" valign="top" onclick="selectRow(67);">50352</td><td align = "left" valign="top" onclick="selectRow(67);">PNR_SORT_ERROR</td><td align = "right" valign="top" onclick="selectRow(67);">21</td><td align = "right" valign="top" onclick="selectRow(67);">13</td><td align = "right" valign="top" onclick="selectRow(67);">20</td>
//	</tr><tr id = "row68" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">69</td><td align = "left" valign="top" onclick="selectRow(68);">50236</td><td align = "left" valign="top" onclick="selectRow(68);">BUNDLE_PURCHASED_ERROR</td><td align = "right" valign="top" onclick="selectRow(68);">20</td><td align = "right" valign="top" onclick="selectRow(68);">1</td><td align = "right" valign="top" onclick="selectRow(68);">2</td>
//	</tr><tr id = "row69" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">70</td><td align = "left" valign="top" onclick="selectRow(69);">50025</td><td align = "left" valign="top" onclick="selectRow(69);">PNR_FLIGHT_HAS_DEPARTED</td><td align = "right" valign="top" onclick="selectRow(69);">19</td><td align = "right" valign="top" onclick="selectRow(69);">16</td><td align = "right" valign="top" onclick="selectRow(69);">41</td>
//	</tr><tr id = "row70" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">71</td><td align = "left" valign="top" onclick="selectRow(70);">50120</td><td align = "left" valign="top" onclick="selectRow(70);">RESYNC_NOT_ELIGIBLE</td><td align = "right" valign="top" onclick="selectRow(70);">19</td><td align = "right" valign="top" onclick="selectRow(70);">25</td><td align = "right" valign="top" onclick="selectRow(70);">56</td>
//	</tr><tr id = "row71" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">72</td><td align = "left" valign="top" onclick="selectRow(71);">50267</td><td align = "left" valign="top" onclick="selectRow(71);">MISSING_DOCUMENT</td><td align = "right" valign="top" onclick="selectRow(71);">19</td><td align = "right" valign="top" onclick="selectRow(71);">18</td><td align = "right" valign="top" onclick="selectRow(71);">8</td>
//	</tr><tr id = "row72" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">73</td><td align = "left" valign="top" onclick="selectRow(72);">50460</td><td align = "left" valign="top" onclick="selectRow(72);">SELFTAG_R2_ACTIVATE_ERROR</td><td align = "right" valign="top" onclick="selectRow(72);">19</td><td align = "right" valign="top" onclick="selectRow(72);">30</td><td align = "right" valign="top" onclick="selectRow(72);">27</td>
//	</tr><tr id = "row73" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">74</td><td align = "left" valign="top" onclick="selectRow(73);">50057</td><td align = "left" valign="top" onclick="selectRow(73);">FLIGHT_STATUS_ERROR</td><td align = "right" valign="top" onclick="selectRow(73);">18</td><td align = "right" valign="top" onclick="selectRow(73);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(73);">&nbsp;</td>
//	</tr><tr id = "row74" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">75</td><td align = "left" valign="top" onclick="selectRow(74);">50315</td><td align = "left" valign="top" onclick="selectRow(74);">INTL_PROFILE_SAVE_ERROR</td><td align = "right" valign="top" onclick="selectRow(74);">18</td><td align = "right" valign="top" onclick="selectRow(74);">37</td><td align = "right" valign="top" onclick="selectRow(74);">41</td>
//	</tr><tr id = "row75" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">76</td><td align = "left" valign="top" onclick="selectRow(75);">50228</td><td align = "left" valign="top" onclick="selectRow(75);">INTL_APIS_SENT_DUPE</td><td align = "right" valign="top" onclick="selectRow(75);">16</td><td align = "right" valign="top" onclick="selectRow(75);">12</td><td align = "right" valign="top" onclick="selectRow(75);">17</td>
//	</tr><tr id = "row76" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">77</td><td align = "left" valign="top" onclick="selectRow(76);">50144</td><td align = "left" valign="top" onclick="selectRow(76);">CSL_FLIFO_LATENCY</td><td align = "right" valign="top" onclick="selectRow(76);">15</td><td align = "right" valign="top" onclick="selectRow(76);">27</td><td align = "right" valign="top" onclick="selectRow(76);">10</td>
//	</tr><tr id = "row77" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">78</td><td align = "left" valign="top" onclick="selectRow(77);">50161</td><td align = "left" valign="top" onclick="selectRow(77);">FQTV_INVALID_NUMBER</td><td align = "right" valign="top" onclick="selectRow(77);">15</td><td align = "right" valign="top" onclick="selectRow(77);">29</td><td align = "right" valign="top" onclick="selectRow(77);">35</td>
//	</tr><tr id = "row78" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">79</td><td align = "left" valign="top" onclick="selectRow(78);">50122</td><td align = "left" valign="top" onclick="selectRow(78);">BAGS_TOO_MANY_FOR_COS</td><td align = "right" valign="top" onclick="selectRow(78);">14</td><td align = "right" valign="top" onclick="selectRow(78);">3</td><td align = "right" valign="top" onclick="selectRow(78);">22</td>
//	</tr><tr id = "row79" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">80</td><td align = "left" valign="top" onclick="selectRow(79);">50085</td><td align = "left" valign="top" onclick="selectRow(79);">CHECKIN_ALREADY_CHECKED_IN</td><td align = "right" valign="top" onclick="selectRow(79);">13</td><td align = "right" valign="top" onclick="selectRow(79);">21</td><td align = "right" valign="top" onclick="selectRow(79);">13</td>
//	</tr><tr id = "row80" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">81</td><td align = "left" valign="top" onclick="selectRow(80);">50141</td><td align = "left" valign="top" onclick="selectRow(80);">PASSPORT_MULTIPLE_NAME_MATCH</td><td align = "right" valign="top" onclick="selectRow(80);">13</td><td align = "right" valign="top" onclick="selectRow(80);">21</td><td align = "right" valign="top" onclick="selectRow(80);">33</td>
//	</tr><tr id = "row81" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">82</td><td align = "left" valign="top" onclick="selectRow(81);">50165</td><td align = "left" valign="top" onclick="selectRow(81);">REACCOM_ERROR</td><td align = "right" valign="top" onclick="selectRow(81);">13</td><td align = "right" valign="top" onclick="selectRow(81);">32</td><td align = "right" valign="top" onclick="selectRow(81);">22</td>
//	</tr><tr id = "row82" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">83</td><td align = "left" valign="top" onclick="selectRow(82);">50016</td><td align = "left" valign="top" onclick="selectRow(82);">PNR_FIRST_SEGMENT_OA</td><td align = "right" valign="top" onclick="selectRow(82);">12</td><td align = "right" valign="top" onclick="selectRow(82);">55</td><td align = "right" valign="top" onclick="selectRow(82);">87</td>
//	</tr><tr id = "row83" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">84</td><td align = "left" valign="top" onclick="selectRow(83);">50176</td><td align = "left" valign="top" onclick="selectRow(83);">ATRE_REPRICE_ADD_COLLECT</td><td align = "right" valign="top" onclick="selectRow(83);">12</td><td align = "right" valign="top" onclick="selectRow(83);">8</td><td align = "right" valign="top" onclick="selectRow(83);">22</td>
//	</tr><tr id = "row84" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">85</td><td align = "left" valign="top" onclick="selectRow(84);">50357</td><td align = "left" valign="top" onclick="selectRow(84);">REACCOM_MISCONNECT</td><td align = "right" valign="top" onclick="selectRow(84);">12</td><td align = "right" valign="top" onclick="selectRow(84);">2</td><td align = "right" valign="top" onclick="selectRow(84);">5</td>
//	</tr><tr id = "row85" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">86</td><td align = "left" valign="top" onclick="selectRow(85);">50260</td><td align = "left" valign="top" onclick="selectRow(85);">FLIGHT_RANGE_PAST_FLIGHT</td><td align = "right" valign="top" onclick="selectRow(85);">11</td><td align = "right" valign="top" onclick="selectRow(85);">52</td><td align = "right" valign="top" onclick="selectRow(85);">20</td>
//	</tr><tr id = "row86" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">87</td><td align = "left" valign="top" onclick="selectRow(86);">50355</td><td align = "left" valign="top" onclick="selectRow(86);">PHONE_SERVICE_ERROR</td><td align = "right" valign="top" onclick="selectRow(86);">11</td><td align = "right" valign="top" onclick="selectRow(86);">16</td><td align = "right" valign="top" onclick="selectRow(86);">27</td>
//	</tr><tr id = "row87" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">88</td><td align = "left" valign="top" onclick="selectRow(87);">50023</td><td align = "left" valign="top" onclick="selectRow(87);">PNR_INTERNATIONAL_FLIGHT</td><td align = "right" valign="top" onclick="selectRow(87);">10</td><td align = "right" valign="top" onclick="selectRow(87);">30</td><td align = "right" valign="top" onclick="selectRow(87);">21</td>
//	</tr><tr id = "row88" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">89</td><td align = "left" valign="top" onclick="selectRow(88);">50087</td><td align = "left" valign="top" onclick="selectRow(88);">CHECKIN_MESSAGES_PNR_RETURNED</td><td align = "right" valign="top" onclick="selectRow(88);">10</td><td align = "right" valign="top" onclick="selectRow(88);">1</td><td align = "right" valign="top" onclick="selectRow(88);">3</td>
//	</tr><tr id = "row89" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">90</td><td align = "left" valign="top" onclick="selectRow(89);">50232</td><td align = "left" valign="top" onclick="selectRow(89);">UPGRADE_STANDBY_FAILED</td><td align = "right" valign="top" onclick="selectRow(89);">10</td><td align = "right" valign="top" onclick="selectRow(89);">7</td><td align = "right" valign="top" onclick="selectRow(89);">6</td>
//	</tr><tr id = "row90" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">91</td><td align = "left" valign="top" onclick="selectRow(90);">50296</td><td align = "left" valign="top" onclick="selectRow(90);">PASSPORT_INVALID_NATIONALITY</td><td align = "right" valign="top" onclick="selectRow(90);">10</td><td align = "right" valign="top" onclick="selectRow(90);">32</td><td align = "right" valign="top" onclick="selectRow(90);">55</td>
//	</tr><tr id = "row91" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">92</td><td align = "left" valign="top" onclick="selectRow(91);">50377</td><td align = "left" valign="top" onclick="selectRow(91);">INVALID_BAGTAG</td><td align = "right" valign="top" onclick="selectRow(91);">10</td><td align = "right" valign="top" onclick="selectRow(91);">66</td><td align = "right" valign="top" onclick="selectRow(91);">64</td>
//	</tr><tr id = "row92" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">93</td><td align = "left" valign="top" onclick="selectRow(92);">50005</td><td align = "left" valign="top" onclick="selectRow(92);">CARD_TYPE_UNKNOWN</td><td align = "right" valign="top" onclick="selectRow(92);">8</td><td align = "right" valign="top" onclick="selectRow(92);">3</td><td align = "right" valign="top" onclick="selectRow(92);">5</td>
//	</tr><tr id = "row93" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">94</td><td align = "left" valign="top" onclick="selectRow(93);">50105</td><td align = "left" valign="top" onclick="selectRow(93);">CHECKIN_FAILED_COR</td><td align = "right" valign="top" onclick="selectRow(93);">8</td><td align = "right" valign="top" onclick="selectRow(93);">17</td><td align = "right" valign="top" onclick="selectRow(93);">17</td>
//	</tr><tr id = "row94" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">95</td><td align = "left" valign="top" onclick="selectRow(94);">50383</td><td align = "left" valign="top" onclick="selectRow(94);">ACI_NAME_SEARCH_ERROR</td><td align = "right" valign="top" onclick="selectRow(94);">8</td><td align = "right" valign="top" onclick="selectRow(94);">8</td><td align = "right" valign="top" onclick="selectRow(94);">8</td>
//	</tr><tr id = "row95" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">96</td><td align = "left" valign="top" onclick="selectRow(95);">50106</td><td align = "left" valign="top" onclick="selectRow(95);">SSR_UNACCOMPANIED_MINOR</td><td align = "right" valign="top" onclick="selectRow(95);">7</td><td align = "right" valign="top" onclick="selectRow(95);">54</td><td align = "right" valign="top" onclick="selectRow(95);">48</td>
//	</tr><tr id = "row96" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">97</td><td align = "left" valign="top" onclick="selectRow(96);">50177</td><td align = "left" valign="top" onclick="selectRow(96);">BOARDINGPASS_FAILED_PRINT</td><td align = "right" valign="top" onclick="selectRow(96);">7</td><td align = "right" valign="top" onclick="selectRow(96);">17</td><td align = "right" valign="top" onclick="selectRow(96);">15</td>
//	</tr><tr id = "row97" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">98</td><td align = "left" valign="top" onclick="selectRow(97);">50193</td><td align = "left" valign="top" onclick="selectRow(97);">KIOSK_REQUIRED_AGENT_OVERRIDE</td><td align = "right" valign="top" onclick="selectRow(97);">7</td><td align = "right" valign="top" onclick="selectRow(97);">29</td><td align = "right" valign="top" onclick="selectRow(97);">41</td>
//	</tr><tr id = "row98" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">99</td><td align = "left" valign="top" onclick="selectRow(98);">70002</td><td align = "left" valign="top" onclick="selectRow(98);">API_NATIVE_HOST_ERROR</td><td align = "right" valign="top" onclick="selectRow(98);">7</td><td align = "right" valign="top" onclick="selectRow(98);">42</td><td align = "right" valign="top" onclick="selectRow(98);">11</td>
//	</tr><tr id = "row99" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">100</td><td align = "left" valign="top" onclick="selectRow(99);">50376</td><td align = "left" valign="top" onclick="selectRow(99);">UA_TO_UA_BAGS</td><td align = "right" valign="top" onclick="selectRow(99);">7</td><td align = "right" valign="top" onclick="selectRow(99);">24</td><td align = "right" valign="top" onclick="selectRow(99);">38</td>
//	</tr><tr id = "row100" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">101</td><td align = "left" valign="top" onclick="selectRow(100);">50059</td><td align = "left" valign="top" onclick="selectRow(100);">KIOSK_SETTING_ERROR</td><td align = "right" valign="top" onclick="selectRow(100);">6</td><td align = "right" valign="top" onclick="selectRow(100);">3</td><td align = "right" valign="top" onclick="selectRow(100);">&nbsp;</td>
//	</tr><tr id = "row101" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">102</td><td align = "left" valign="top" onclick="selectRow(101);">50149</td><td align = "left" valign="top" onclick="selectRow(101);">SSR_MEDA</td><td align = "right" valign="top" onclick="selectRow(101);">6</td><td align = "right" valign="top" onclick="selectRow(101);">1</td><td align = "right" valign="top" onclick="selectRow(101);">&nbsp;</td>
//	</tr><tr id = "row102" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">103</td><td align = "left" valign="top" onclick="selectRow(102);">50213</td><td align = "left" valign="top" onclick="selectRow(102);">ONEPASS_CUSTOMER_DB_ERROR</td><td align = "right" valign="top" onclick="selectRow(102);">6</td><td align = "right" valign="top" onclick="selectRow(102);">1</td><td align = "right" valign="top" onclick="selectRow(102);">3</td>
//	</tr><tr id = "row103" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">104</td><td align = "left" valign="top" onclick="selectRow(103);">50233</td><td align = "left" valign="top" onclick="selectRow(103);">BAG_PARTNER_ERROR</td><td align = "right" valign="top" onclick="selectRow(103);">6</td><td align = "right" valign="top" onclick="selectRow(103);">20</td><td align = "right" valign="top" onclick="selectRow(103);">10</td>
//	</tr><tr id = "row104" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">105</td><td align = "left" valign="top" onclick="selectRow(104);">50321</td><td align = "left" valign="top" onclick="selectRow(104);">SELFTAG_ACTIVATE_ERROR</td><td align = "right" valign="top" onclick="selectRow(104);">6</td><td align = "right" valign="top" onclick="selectRow(104);">10</td><td align = "right" valign="top" onclick="selectRow(104);">11</td>
//	</tr><tr id = "row105" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">106</td><td align = "left" valign="top" onclick="selectRow(105);">50443</td><td align = "left" valign="top" onclick="selectRow(105);">GATEBAG_SERVICE_ERROR</td><td align = "right" valign="top" onclick="selectRow(105);">6</td><td align = "right" valign="top" onclick="selectRow(105);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(105);">&nbsp;</td>
//	</tr><tr id = "row106" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">107</td><td align = "left" valign="top" onclick="selectRow(106);">50416</td><td align = "left" valign="top" onclick="selectRow(106);">UPGRADE_PLATFORM_ERROR</td><td align = "right" valign="top" onclick="selectRow(106);">6</td><td align = "right" valign="top" onclick="selectRow(106);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(106);">5</td>
//	</tr><tr id = "row107" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">108</td><td align = "left" valign="top" onclick="selectRow(107);">50243</td><td align = "left" valign="top" onclick="selectRow(107);">BARCODE_READ_ERROR</td><td align = "right" valign="top" onclick="selectRow(107);">5</td><td align = "right" valign="top" onclick="selectRow(107);">18</td><td align = "right" valign="top" onclick="selectRow(107);">18</td>
//	</tr><tr id = "row108" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">109</td><td align = "left" valign="top" onclick="selectRow(108);">50318</td><td align = "left" valign="top" onclick="selectRow(108);">SELFTAG_DELETE_ERROR</td><td align = "right" valign="top" onclick="selectRow(108);">5</td><td align = "right" valign="top" onclick="selectRow(108);">15</td><td align = "right" valign="top" onclick="selectRow(108);">12</td>
//	</tr><tr id = "row109" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">110</td><td align = "left" valign="top" onclick="selectRow(109);">50361</td><td align = "left" valign="top" onclick="selectRow(109);">CSL_FLIFO_ERROR</td><td align = "right" valign="top" onclick="selectRow(109);">5</td><td align = "right" valign="top" onclick="selectRow(109);">8</td><td align = "right" valign="top" onclick="selectRow(109);">12</td>
//	</tr><tr id = "row110" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">111</td><td align = "left" valign="top" onclick="selectRow(110);">50010</td><td align = "left" valign="top" onclick="selectRow(110);">PRINTDOC_ERROR</td><td align = "right" valign="top" onclick="selectRow(110);">4</td><td align = "right" valign="top" onclick="selectRow(110);">7</td><td align = "right" valign="top" onclick="selectRow(110);">4</td>
//	</tr><tr id = "row111" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">112</td><td align = "left" valign="top" onclick="selectRow(111);">70024</td><td align = "left" valign="top" onclick="selectRow(111);">API_CHANGE_FLIGHT_NO_LONGER_AVAILABLE</td><td align = "right" valign="top" onclick="selectRow(111);">4</td><td align = "right" valign="top" onclick="selectRow(111);">1</td><td align = "right" valign="top" onclick="selectRow(111);">2</td>
//	</tr><tr id = "row112" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">113</td><td align = "left" valign="top" onclick="selectRow(112);">50459</td><td align = "left" valign="top" onclick="selectRow(112);">SELFTAG_R2_PRINT_ERROR</td><td align = "right" valign="top" onclick="selectRow(112);">4</td><td align = "right" valign="top" onclick="selectRow(112);">9</td><td align = "right" valign="top" onclick="selectRow(112);">12</td>
//	</tr><tr id = "row113" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">114</td><td align = "left" valign="top" onclick="selectRow(113);">50125</td><td align = "left" valign="top" onclick="selectRow(113);">CHANGE_FLIGHT_NO_LONGER_AVAILABLE</td><td align = "right" valign="top" onclick="selectRow(113);">4</td><td align = "right" valign="top" onclick="selectRow(113);">1</td><td align = "right" valign="top" onclick="selectRow(113);">1</td>
//	</tr><tr id = "row114" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">115</td><td align = "left" valign="top" onclick="selectRow(114);">70007</td><td align = "left" valign="top" onclick="selectRow(114);">API_ADD_SSR_ERROR</td><td align = "right" valign="top" onclick="selectRow(114);">4</td><td align = "right" valign="top" onclick="selectRow(114);">20</td><td align = "right" valign="top" onclick="selectRow(114);">23</td>
//	</tr><tr id = "row115" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">116</td><td align = "left" valign="top" onclick="selectRow(115);">50286</td><td align = "left" valign="top" onclick="selectRow(115);">BAGS_REISSUE_FAILED</td><td align = "right" valign="top" onclick="selectRow(115);">4</td><td align = "right" valign="top" onclick="selectRow(115);">3</td><td align = "right" valign="top" onclick="selectRow(115);">3</td>
//	</tr><tr id = "row116" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">117</td><td align = "left" valign="top" onclick="selectRow(116);">50439</td><td align = "left" valign="top" onclick="selectRow(116);">RAVEN_SERVICE_ERROR</td><td align = "right" valign="top" onclick="selectRow(116);">4</td><td align = "right" valign="top" onclick="selectRow(116);">2</td><td align = "right" valign="top" onclick="selectRow(116);">5</td>
//	</tr><tr id = "row117" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">118</td><td align = "left" valign="top" onclick="selectRow(117);">50019</td><td align = "left" valign="top" onclick="selectRow(117);">PNR_CUSTOMER_NOT_FOUND</td><td align = "right" valign="top" onclick="selectRow(117);">3</td><td align = "right" valign="top" onclick="selectRow(117);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(117);">&nbsp;</td>
//	</tr><tr id = "row118" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">119</td><td align = "left" valign="top" onclick="selectRow(118);">50052</td><td align = "left" valign="top" onclick="selectRow(118);">SCHEDULE_ENGINE_ERROR</td><td align = "right" valign="top" onclick="selectRow(118);">3</td><td align = "right" valign="top" onclick="selectRow(118);">4</td><td align = "right" valign="top" onclick="selectRow(118);">3</td>
//	</tr><tr id = "row119" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">120</td><td align = "left" valign="top" onclick="selectRow(119);">50054</td><td align = "left" valign="top" onclick="selectRow(119);">RESYNC_SHARES_ERROR</td><td align = "right" valign="top" onclick="selectRow(119);">3</td><td align = "right" valign="top" onclick="selectRow(119);">1</td><td align = "right" valign="top" onclick="selectRow(119);">&nbsp;</td>
//	</tr><tr id = "row120" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">121</td><td align = "left" valign="top" onclick="selectRow(120);">70030</td><td align = "left" valign="top" onclick="selectRow(120);">API_RESYNC_FAILED</td><td align = "right" valign="top" onclick="selectRow(120);">3</td><td align = "right" valign="top" onclick="selectRow(120);">1</td><td align = "right" valign="top" onclick="selectRow(120);">&nbsp;</td>
//	</tr><tr id = "row121" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">122</td><td align = "left" valign="top" onclick="selectRow(121);">50270</td><td align = "left" valign="top" onclick="selectRow(121);">BARCODE_DIGITAL_SIGNATURE_ERROR</td><td align = "right" valign="top" onclick="selectRow(121);">3</td><td align = "right" valign="top" onclick="selectRow(121);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(121);">&nbsp;</td>
//	</tr><tr id = "row122" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">123</td><td align = "left" valign="top" onclick="selectRow(122);">50288</td><td align = "left" valign="top" onclick="selectRow(122);">GAIN_CONTROL_ERROR</td><td align = "right" valign="top" onclick="selectRow(122);">3</td><td align = "right" valign="top" onclick="selectRow(122);">5</td><td align = "right" valign="top" onclick="selectRow(122);">7</td>
//	</tr><tr id = "row123" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">124</td><td align = "left" valign="top" onclick="selectRow(123);">50302</td><td align = "left" valign="top" onclick="selectRow(123);">UMNR_READ_ERROR</td><td align = "right" valign="top" onclick="selectRow(123);">3</td><td align = "right" valign="top" onclick="selectRow(123);">1</td><td align = "right" valign="top" onclick="selectRow(123);">1</td>
//	</tr><tr id = "row124" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">125</td><td align = "left" valign="top" onclick="selectRow(124);">50363</td><td align = "left" valign="top" onclick="selectRow(124);">BCC_SCAN_ERROR</td><td align = "right" valign="top" onclick="selectRow(124);">3</td><td align = "right" valign="top" onclick="selectRow(124);">2</td><td align = "right" valign="top" onclick="selectRow(124);">6</td>
//	</tr><tr id = "row125" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">126</td><td align = "left" valign="top" onclick="selectRow(125);">50003</td><td align = "left" valign="top" onclick="selectRow(125);">DB_MACHINE_NAME_NOT_FOUND</td><td align = "right" valign="top" onclick="selectRow(125);">2</td><td align = "right" valign="top" onclick="selectRow(125);">4</td><td align = "right" valign="top" onclick="selectRow(125);">10</td>
//	</tr><tr id = "row126" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">127</td><td align = "left" valign="top" onclick="selectRow(126);">50093</td><td align = "left" valign="top" onclick="selectRow(126);">CHECKIN_MESSAGES_SECURITY_PROFILE_ERROR</td><td align = "right" valign="top" onclick="selectRow(126);">2</td><td align = "right" valign="top" onclick="selectRow(126);">2</td><td align = "right" valign="top" onclick="selectRow(126);">2</td>
//	</tr><tr id = "row127" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">128</td><td align = "left" valign="top" onclick="selectRow(127);">50107</td><td align = "left" valign="top" onclick="selectRow(127);">SSR_OXYGEN_REQUIRED</td><td align = "right" valign="top" onclick="selectRow(127);">2</td><td align = "right" valign="top" onclick="selectRow(127);">1</td><td align = "right" valign="top" onclick="selectRow(127);">&nbsp;</td>
//	</tr><tr id = "row128" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">129</td><td align = "left" valign="top" onclick="selectRow(128);">50121</td><td align = "left" valign="top" onclick="selectRow(128);">CABIN_BRANDING_ERROR</td><td align = "right" valign="top" onclick="selectRow(128);">2</td><td align = "right" valign="top" onclick="selectRow(128);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(128);">&nbsp;</td>
//	</tr><tr id = "row129" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">130</td><td align = "left" valign="top" onclick="selectRow(129);">50198</td><td align = "left" valign="top" onclick="selectRow(129);">CHANGE_CABIN_FAILED</td><td align = "right" valign="top" onclick="selectRow(129);">2</td><td align = "right" valign="top" onclick="selectRow(129);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(129);">&nbsp;</td>
//	</tr><tr id = "row130" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">131</td><td align = "left" valign="top" onclick="selectRow(130);">50264</td><td align = "left" valign="top" onclick="selectRow(130);">AGENT_OVERRIDE_REQUIRED</td><td align = "right" valign="top" onclick="selectRow(130);">2</td><td align = "right" valign="top" onclick="selectRow(130);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(130);">2</td>
//	</tr><tr id = "row131" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">132</td><td align = "left" valign="top" onclick="selectRow(131);">50317</td><td align = "left" valign="top" onclick="selectRow(131);">SELFTAG_HOLD_ERROR</td><td align = "right" valign="top" onclick="selectRow(131);">2</td><td align = "right" valign="top" onclick="selectRow(131);">2</td><td align = "right" valign="top" onclick="selectRow(131);">4</td>
//	</tr><tr id = "row132" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">133</td><td align = "left" valign="top" onclick="selectRow(132);">70013</td><td align = "left" valign="top" onclick="selectRow(132);">API_CHANGE_CABIN_FAILED</td><td align = "right" valign="top" onclick="selectRow(132);">2</td><td align = "right" valign="top" onclick="selectRow(132);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(132);">&nbsp;</td>
//	</tr><tr id = "row133" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">134</td><td align = "left" valign="top" onclick="selectRow(133);">50339</td><td align = "left" valign="top" onclick="selectRow(133);">INVALID_SEGMENT</td><td align = "right" valign="top" onclick="selectRow(133);">2</td><td align = "right" valign="top" onclick="selectRow(133);">3</td><td align = "right" valign="top" onclick="selectRow(133);">&nbsp;</td>
//	</tr><tr id = "row134" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">135</td><td align = "left" valign="top" onclick="selectRow(134);">50395</td><td align = "left" valign="top" onclick="selectRow(134);">EMD_ISSUE_ERROR</td><td align = "right" valign="top" onclick="selectRow(134);">2</td><td align = "right" valign="top" onclick="selectRow(134);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(134);">1</td>
//	</tr><tr id = "row135" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">136</td><td align = "left" valign="top" onclick="selectRow(135);">50001</td><td align = "left" valign="top" onclick="selectRow(135);">DB_STATE_ERROR</td><td align = "right" valign="top" onclick="selectRow(135);">1</td><td align = "right" valign="top" onclick="selectRow(135);">2</td><td align = "right" valign="top" onclick="selectRow(135);">3</td>
//	</tr><tr id = "row136" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">137</td><td align = "left" valign="top" onclick="selectRow(136);">70023</td><td align = "left" valign="top" onclick="selectRow(136);">API_FLIFO_ERROR</td><td align = "right" valign="top" onclick="selectRow(136);">1</td><td align = "right" valign="top" onclick="selectRow(136);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(136);">&nbsp;</td>
//	</tr><tr id = "row137" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">138</td><td align = "left" valign="top" onclick="selectRow(137);">50145</td><td align = "left" valign="top" onclick="selectRow(137);">OVERSALE_SERVICE_FAILED</td><td align = "right" valign="top" onclick="selectRow(137);">1</td><td align = "right" valign="top" onclick="selectRow(137);">1</td><td align = "right" valign="top" onclick="selectRow(137);">&nbsp;</td>
//	</tr><tr id = "row138" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">139</td><td align = "left" valign="top" onclick="selectRow(138);">50152</td><td align = "left" valign="top" onclick="selectRow(138);">INTL_APIS_HOST_LINK_DOWN</td><td align = "right" valign="top" onclick="selectRow(138);">1</td><td align = "right" valign="top" onclick="selectRow(138);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(138);">3</td>
//	</tr><tr id = "row139" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">140</td><td align = "left" valign="top" onclick="selectRow(139);">50184</td><td align = "left" valign="top" onclick="selectRow(139);">ATRE_FARE_RULES_FAILED</td><td align = "right" valign="top" onclick="selectRow(139);">1</td><td align = "right" valign="top" onclick="selectRow(139);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(139);">&nbsp;</td>
//	</tr><tr id = "row140" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">141</td><td align = "left" valign="top" onclick="selectRow(140);">50199</td><td align = "left" valign="top" onclick="selectRow(140);">DIVIDE_PNR_FAILED</td><td align = "right" valign="top" onclick="selectRow(140);">1</td><td align = "right" valign="top" onclick="selectRow(140);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(140);">&nbsp;</td>
//	</tr><tr id = "row141" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">142</td><td align = "left" valign="top" onclick="selectRow(141);">52002</td><td align = "left" valign="top" onclick="selectRow(141);">KIOSK_PSD_ERROR</td><td align = "right" valign="top" onclick="selectRow(141);">1</td><td align = "right" valign="top" onclick="selectRow(141);">3</td><td align = "right" valign="top" onclick="selectRow(141);">3</td>
//	</tr><tr id = "row142" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">143</td><td align = "left" valign="top" onclick="selectRow(142);">50223</td><td align = "left" valign="top" onclick="selectRow(142);">AJAX_ERROR</td><td align = "right" valign="top" onclick="selectRow(142);">1</td><td align = "right" valign="top" onclick="selectRow(142);">3</td><td align = "right" valign="top" onclick="selectRow(142);">4</td>
//	</tr><tr id = "row143" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">144</td><td align = "left" valign="top" onclick="selectRow(143);">50275</td><td align = "left" valign="top" onclick="selectRow(143);">BAG_SERVICE_ERROR</td><td align = "right" valign="top" onclick="selectRow(143);">1</td><td align = "right" valign="top" onclick="selectRow(143);">2</td><td align = "right" valign="top" onclick="selectRow(143);">4</td>
//	</tr><tr id = "row144" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">145</td><td align = "left" valign="top" onclick="selectRow(144);">50325</td><td align = "left" valign="top" onclick="selectRow(144);">BAGS_DELETE_ERROR</td><td align = "right" valign="top" onclick="selectRow(144);">1</td><td align = "right" valign="top" onclick="selectRow(144);">4</td><td align = "right" valign="top" onclick="selectRow(144);">3</td>
//	</tr><tr id = "row145" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">146</td><td align = "left" valign="top" onclick="selectRow(145);">50330</td><td align = "left" valign="top" onclick="selectRow(145);">SELFTAG_NUMBER_ERROR</td><td align = "right" valign="top" onclick="selectRow(145);">1</td><td align = "right" valign="top" onclick="selectRow(145);">3</td><td align = "right" valign="top" onclick="selectRow(145);">1</td>
//	</tr><tr id = "row146" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">147</td><td align = "left" valign="top" onclick="selectRow(146);">50341</td><td align = "left" valign="top" onclick="selectRow(146);">SEARCH_INVALID_LASTNAME</td><td align = "right" valign="top" onclick="selectRow(146);">1</td><td align = "right" valign="top" onclick="selectRow(146);">1</td><td align = "right" valign="top" onclick="selectRow(146);">4</td>
//	</tr><tr id = "row147" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">148</td><td align = "left" valign="top" onclick="selectRow(147);">50401</td><td align = "left" valign="top" onclick="selectRow(147);">MERCH_PURCHASE_ERROR</td><td align = "right" valign="top" onclick="selectRow(147);">1</td><td align = "right" valign="top" onclick="selectRow(147);">1</td><td align = "right" valign="top" onclick="selectRow(147);">1</td>
//	</tr><tr id = "row148" class="tblItem">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">149</td><td align = "left" valign="top" onclick="selectRow(148);">50419</td><td align = "left" valign="top" onclick="selectRow(148);">CLOUD_TOKEN_ERROR</td><td align = "right" valign="top" onclick="selectRow(148);">1</td><td align = "right" valign="top" onclick="selectRow(148);">&nbsp;</td><td align = "right" valign="top" onclick="selectRow(148);">&nbsp;</td>
//	</tr><tr id = "row149" class="tblItemAlternating">
//		<td align = "center" valign="top" style="width:30px;white-space:nowrap;">150</td><td align = "left" valign="top" onclick="selectRow(149);">50426</td><td align = "left" valign="top" onclick="selectRow(149);">REFERENCEDATA_GMT_ERROR</td><td align = "right" valign="top" onclick="selectRow(149);">1</td><td align = "right" valign="top" onclick="selectRow(149);">11</td><td align = "right" valign="top" onclick="selectRow(149);">2</td>
//	</tr>
//</tbody></table>
    }
}
