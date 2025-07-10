'use client'

import Image from 'next/image'
import cn from 'classnames'
import { useRef } from 'react'

import { useGlowingAnimation } from '@/shared/hooks'

import classes from './UpgradeCentralInstallation.module.scss'

type UpgradeState = 'default' | 'lose' | 'win'

type UpgradeCentralInstallationProps = {
  state: UpgradeState
  glowing: boolean
  explosion: boolean
}

export const UpgradeCentralInstallation = ({
  state,
  glowing,
  explosion
}: UpgradeCentralInstallationProps) => {
  const loseImageRef = useRef<HTMLImageElement>(null)
  const winImageRef = useRef<HTMLImageElement>(null)

  useGlowingAnimation({
    elementsRef: [winImageRef, loseImageRef],
    enabled: glowing
  })

  const getUpgradeStateImage = (state: UpgradeState) => {
    switch (state) {
      case 'default':
        return '/img/upgrades/upgrade-default-state.svg'
      case 'lose':
        return '/img/upgrades/upgrade-lose.svg'
      case 'win':
        return '/img/upgrades/upgrade-win.svg'
    }
  }

  return (
    <div className={classes.upgradeCentralInstallation}>
      <Image
        src='/img/upgrades/upgrade-device-main-bg.png'
        width={781}
        height={847}
        quality={100}
        priority
        alt='Главный фон'
      />

      <div
        className={cn(classes.stateImg, {
          [classes.resultImgVisible]: glowing
        })}
      >
        <Image
          src={getUpgradeStateImage(state)}
          width={235}
          height={235}
          quality={100}
          priority
          alt='Состояние'
        />

        <Image
          ref={winImageRef}
          src='/img/upgrades/upgrade-win-without-text.svg'
          width={235}
          height={235}
          quality={100}
          alt='Состояние'
        />

        <Image
          ref={loseImageRef}
          src='/img/upgrades/upgrade-lose-without-text.svg'
          width={235}
          height={235}
          quality={100}
          alt='Состояние'
        />

        {explosion ? (
          <Image
            className={classes.explosionImg}
            src='/img/upgrades/explosion.gif'
            width={344}
            height={353}
            alt='Взрыв'
            unoptimized
          />
        ) : null}
      </div>
    </div>
  )
}
