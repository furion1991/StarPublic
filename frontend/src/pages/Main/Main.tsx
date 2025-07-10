import Image from 'next/image'
import Link from 'next/link'

import { Button } from '@/shared/ui'
import { HeroicBundlesCasesList, PopularCasesList } from '@/entities/cases'
import { TotalSiteStatistics } from '@/entities/stats'

import classes from './Main.module.scss'

export const MainPage = () => {
  return (
    <div className={classes.mainPage}>
      <div className={classes.header}>
        <div className={classes.bgImage}>
          <Image
            src='/img/main-header-bg.png'
            width={1920}
            height={1080}
            priority
            quality={100}
            alt='Фон'
          />
        </div>

        <div className={classes.headerTitle}>
          <Image
            className={classes.headerTitleImg}
            src='/img/main-header-title.png'
            width={1232}
            height={167}
            priority
            quality={100}
            alt='Заголовок'
          />

          <Image
            className={classes.headerTitleMobileImg}
            src='/img/main-header-title-mobile.png'
            width={323}
            height={104}
            priority
            quality={100}
            alt='Заголовок'
          />
        </div>

        <Link href='/' className={classes.tryNewModeBtn}>
          <Button color='purple'>Попробовать</Button>
        </Link>
      </div>

      <div className={classes.content}>
        <section className={classes.popularCases}>
          <div className={classes.sectionHeading}>
            <h4>Популярные кейсы</h4>
            <p>Забери AWP Asimov за минуту</p>
          </div>

          <div className={classes.popularCasesList}>
            <PopularCasesList />
          </div>
        </section>

        <section className={classes.heroicBundles}>
          <div className={classes.sectionHeading}>
            <h4>Героические наборы</h4>
            <p>Забери AWP Asimov за минуту</p>
          </div>

          <div className={classes.heroicBundlesList}>
            <HeroicBundlesCasesList />
          </div>
        </section>

        <div className={classes.totalStatistics}>
          <TotalSiteStatistics />
        </div>
      </div>
    </div>
  )
}
