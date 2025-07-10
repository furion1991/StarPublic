import { CurrentOnlineNumber } from '@/entities/stats'
import { UserEntry } from '@/entities/auth'
import { CrashRocketLink, Logo, SocialNetworksLinksList } from '@/shared/ui'
import { LatestOpenCases } from '@/widgets/cases'

import { NavList } from '../NavList/NavList'

import classes from './Header.module.scss'

export const Header = () => {
  return (
    <header className={classes.header}>
      <div className={classes.desktopLayout}>
        <div className={classes.topLine}>
          <div className={classes.logo}>
            <Logo withShadow />
          </div>

          <div className={classes.latestOpenCases}>
            <LatestOpenCases />
          </div>

          <div className={classes.currentOnlineNumber}>
            <CurrentOnlineNumber />
          </div>
        </div>

        <div className={classes.bottomLine}>
          <NavList />

          <div className={classes.bottomRight}>
            {/* <CrashRocketLink className={classes.crashRocketLink} /> */}

            <div className={classes.socialNetworksList}>
              <SocialNetworksLinksList />
            </div>

            <div className={classes.userEntry}>
              <UserEntry />
            </div>
          </div>
        </div>
      </div>

      <div className={classes.mobileLayout}>
        <div className={classes.topLine}>
          <div className={classes.logo}>
            <Logo withShadow />
          </div>

          <div className={classes.ellipseLeft} />
          <div className={classes.ellipseRight} />

          <div className={classes.topLineRight}>
            <div className={classes.currentOnlineNumber}>
              <CurrentOnlineNumber />
            </div>

            <div className={classes.userEntry}>
              <UserEntry />
            </div>
          </div>
        </div>

        <div className={classes.bottomLine}>
          <div className={classes.latestOpenCases}>
            <LatestOpenCases />
          </div>
        </div>
      </div>
    </header>
  )
}
