import Link from 'next/link'
import Image from 'next/image'
import cn from 'classnames'

import classes from './CrashRocketLink.module.scss'

type CrashRocketLinkProps = {
  className?: string
  rounded?: boolean
}

export const CrashRocketLink = ({ className, rounded }: CrashRocketLinkProps) => {
  return (
    <Link
      href='/crash-rocket'
      className={cn(classes.crashRocketLink, className, {
        [classes.rounded]: rounded
      })}
    >
      <Image
        className={classes.bg}
        src='/img/crash-rocket-banner-bg.png'
        alt='Фон'
        quality={100}
        fill
      />

      <Image
        className={classes.girlImg}
        src='/img/crash-rocket-banner-girl.png'
        alt='Девочка'
        quality={100}
        fill
      />

      <span>
        Краш <br /> Ракета
      </span>
    </Link>
  )
}
