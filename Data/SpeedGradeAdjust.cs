/*
Copyright (C) 2009 Brendan Doherty

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace TrailsPlugin.Data
{
    //Some hints here: http://mymarathonpace.com/Other_Info.html

    public enum RunningGradeAdjustMethodEnum { None, MervynDavies, GregMaclin, JackDaniels, AlbertoMinetti, ACSM, Pandolf, Last };
    public static class RunningGradeAdjustMethodClass
    {
        public static float getGradeFactor(float g, float time, float prevTime, float dist, float prevDist)
        {
            //g = Math.Min(g, 0.2F);
            //g = Math.Max(g, -0.2F);

            float q;
            switch (TrailsPlugin.Data.Settings.RunningGradeAdjustMethod)
            {
                case RunningGradeAdjustMethodEnum.None:
                    q = 1;
                    break;

                case RunningGradeAdjustMethodEnum.MervynDavies:
                case RunningGradeAdjustMethodEnum.GregMaclin:
                    //Mervyn Davies, Greg Maclin http://runningtimes.com/Article.aspx?ArticleID=10507 
                    /* First, I tried to find out if anyone had done any serious research into the subject and ran across this article from Running Times: http://runningtimes.com/Article.aspx?ArticleID=10507 
                     * This article mentions a researcher named Mervyn Davies who did extensive studies in the early 1980's of the effect of hills on the energy expenditure of runners. 
                     * (Tim Noakes quotes Davies' research in his book the "Lore of Running".) Mervyn Davies' basic forumla for how much energy runners expend on hills is as follows: 
                     * Every 1% of upgrade slows your pace 3.3% Every 1% of downgrade speeds your pace by 1.8% After playing around with this formula on elevation data for various marathons, 
                     * I felt it needed to be "tweaked" a little for the much longer distance. Here is what I came up with: Miles 1 - 16: Every 1% of upgrade slows your pace 3.3% 
                     * Every 1% of downgrade speeds your pace by 1.8% Miles 16 - 21: Every 1% of upgrade slows your pace 3.8% Every 1% of downgrade speeds your pace by 1.8% 
                     * Miles 21 - 26.2: Every 1% of upgrade slows your pace 4.3% Every 1% of downgrade speeds your pace by 1.8% (The logic behind this is that uphills located late in 
                     * the race will be more difficult than uphills located closer to the start.)
                     */
                    float q_md;
                    float q_gm;
                    if (g > 0)
                    {
                        //Some adjustment, does not work well over 10%
                        if (g > 0.2)
                        {
                            q_md = 0.5F;
                            q_gm = 0.5F;
                        }
                        else
                        {
                            q_md = 3.3F;
                            q_gm = 3.3F;
                            if (dist > 21 * 1609)
                            {
                                q_gm = 4.3F;
                            }
                            else if (dist > 16 * 1609)
                            {
                                q_gm = 3.8F;
                            }

                            if (g > 0.1)
                            {
                                q_md = (float)(0.1 * (g - 0.1) / 2) * q_md;
                                q_gm = (float)(0.1 * (g - 0.1) / 2) * q_gm;
                            }
                            else
                            {
                                q_md = g * q_md;
                                q_gm = g * q_gm;
                            }
                        }
                    }
                    else
                    {
                        q_md = g * 1.8F;
                        q_gm = 1.8F;
                        if (dist > 21 * 1609)
                        {
                            q_gm = 1.7F;
                        }
                        q_gm *= g;
                    }

                    if (Settings.RunningGradeAdjustMethod == RunningGradeAdjustMethodEnum.MervynDavies)
                    {
                        q = 1 - q_md;
                    }
                    else
                    {
                        q = 1 - q_gm;
                    }
                    break;

                case RunningGradeAdjustMethodEnum.JackDaniels:
                    //Jack Daniels (jtupper)
                    //http://www.letsrun.com/forum/flat_read.php?thread=197366&page=0#ixzz23qCNy3uo
                    /* I also have formulas that make both up and downhill conversions in some programmable calculator somewhere around here. Seems that each % up hill slows you about 15 sec per mile 
                     * and each % down gives you about 8 seconds per mile benefit, provided that you maintain the same energy expenditure. Another calculation I had showed about 12 sec per mile lost per % up. 
                     * I've been tryng to get some runners to do another study on this. It isn't hard to do -- just run some repeated 5-min runs at different grades and calculate how big an increase you get 
                     * in VO2 with each % grade. When we did this before I remember that different people respond differently -- some handle hills better than others (who may handle speed increases better 
                     * than the hill people). A problem is that the up grade increases the cost so much that it is hard to run very fast, because the VO2 will go above max real quickly. So yu end up 
                     * extrapolating from slower speeds and hope it applies at faster ones. I have done faster ones using Rate of Perceived Exertion and that can be done beyond max, but not ver exact
                     * */
                    float q_jd = 0;
                    float paceMile = (time - prevTime) * 1609 / (dist - prevDist) / 100F;
                    //const float maxPace = 8 * 60 / 1000F * 1.609F;
                    //const float minPace = 3 * 60 / 1000F * 1.609F;
                    //paceMile = Math.Min(paceMile, maxPace);
                    //paceMile = Math.Max(paceMile, minPace);
                    if (g > 0)
                    {
                        q_jd = 15 / paceMile;
                    }
                    else
                    {
                        q_jd = 8 / paceMile;
                    }
                    q = 1 - q_jd * g;
                    break;

                case RunningGradeAdjustMethodEnum.AlbertoMinetti:
                    //Energy cost of walking and running at extreme uphill and downhill slopes
                    //Alberto E. Minetti, Christian Moia1, Giulio S. Roi, Davide Susta1, and Guido Ferretti
                    //http://jap.physiology.org/content/93/3/1039.full
                    float q_am0 = (float)((g * (19.5 + g * (46.3 + g * (-43.3 + g * (-30.4 + g * 155.4))))) / 3.6);
                    float q_am1 = 1 / (1 + q_am0);
                    q = q_am1;
                    break;

                case RunningGradeAdjustMethodEnum.ACSM:
                    //http://www.edulife.com.br/dados%5CArtigos%5CEducacao%20Fisica%5CFisiologia%20do%20Exercicio%5CEnergy%20expenditure%20of%20walking%20and%20running%20comparison%20with%20prrediction%20equations.pdf
                    //Running. V˙ O2 (mL·kg1·min1) = 0.2v +  0.9vg*100+  3.5
                    //vflat=v*(1+4.5*g)
                    q = 1 / (1 + 4.5F * g);
                    break;

                case RunningGradeAdjustMethodEnum.Pandolf:
                    //Pandolf, adjusted for running by Epstein (has no impact without load)
                    //http://ftp.rta.nato.int/public//PubFullText/RTO/TR/RTO-TR-HFM-080///TR-HFM-080-03.pdf
                    //http://www.springerlink.com/content/x372781w776h3367/
                    //Mw=1.5 M + 2.0 (M + L)(L/M)^2 + n(M + L)[1.5V^2 + 0.35VG] = M(1.5 +[1.5V^2 + 0.35VG])=1.5M(1+v^2+7/30vg)
                    //Mr = Mw - 0.5 • (1-001 • L) • (Mw -15 • L - 850)=Mw-0.5*(Mw-850)
                    //float q_p = 1 / (float)Math.Sqrt(1 + 0.7 / 3 * (elap - prevElap) * g / (dist - prevDist));
                    float q_p = 1 + 60F * 7F / 30F * g * (time - prevTime) / (dist - prevDist);
                    if (q_p < 0)
                    {
                        q_p = 1;
                    }
                    else
                    {
                        q_p = 1 / (float)Math.Sqrt(q_p);
                    }
                    q = q_p;
                    break;

                default:
                    q = 1;
                    break;
            }
            if (float.IsNaN(q) || q <= 0)
            {
                q = 0;
            }

            return q;
        }
    }
}
