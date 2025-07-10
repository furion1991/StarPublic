import Image from 'next/image'
import cn from 'classnames'
import { type RefObject, useRef } from 'react'

import { useGlowingAnimation } from '@/shared/hooks'

import type { UfoColor } from '../../types/upgrades.types'

import classes from './Ufo.module.scss'

type Direction = 'left' | 'right'

type UfoProps = {
  color: UfoColor
  direction: Direction
  glowing: boolean
}

type UfoLightsProps = {
  color: UfoColor
  hidden: boolean
}

const UfoLights = ({ color, hidden }: UfoLightsProps) => {
  return (
    <>
      <svg
        className={cn(classes.ufoLight, {
          [classes.ufoLightHidden]: hidden
        })}
        width='193'
        height='249'
        viewBox='0 0 193 249'
        fill='none'
        xmlns='http://www.w3.org/2000/svg'
      >
        <path
          d='M187.783 218.889L183.5 218.5L37.5 0.5L0.5 92L154 241.5L153.238 244.547C153.083 245.166 153.128 245.819 153.365 246.412C153.759 247.398 154.639 248.107 155.687 248.281L158.268 248.711C159.408 248.901 160.576 248.832 161.685 248.508L168.603 246.491C170.863 245.832 173.059 244.97 175.165 243.917L175.385 243.808C177.789 242.605 180.073 241.175 182.204 239.535L183.605 238.458C184.866 237.488 186.038 236.408 187.108 235.231L188.496 233.704C189.821 232.247 190.863 230.557 191.569 228.72L192.517 226.257C192.834 225.432 192.943 224.541 192.833 223.664C192.623 221.987 191.636 220.507 190.169 219.668L190.061 219.606C189.362 219.207 188.585 218.962 187.783 218.889Z'
          fill={`url(#ufo-light-left-${color})`}
        />
        <linearGradient
          id={`ufo-light-left-${color}`}
          x1='18'
          y1='80.5'
          x2='173.5'
          y2='232.5'
          gradientUnits='userSpaceOnUse'
        >
          <stop stopColor='#090D31' />
          <stop offset='0.287271' stopColor='#090D31' stopOpacity='0.45' />
          <stop className={cn(classes.ufoLightStop, classes[color])} offset='1' stopOpacity='0.5' />
        </linearGradient>
      </svg>

      <svg
        className={cn(classes.ufoLight, {
          [classes.ufoLightHidden]: hidden
        })}
        width='166'
        height='303'
        viewBox='0 0 166 303'
        fill='none'
        xmlns='http://www.w3.org/2000/svg'
      >
        <path
          d='M3.28824 271.712L7 268L89.5 0L166 42.5L45.1892 282.636C44.4201 284.165 44.2028 285.913 44.574 287.583C44.8532 288.839 44.801 290.147 44.4225 291.377L44.0172 292.694C43.3513 294.858 42.116 296.804 40.4405 298.327L40.052 298.68C38.3768 300.203 36.3505 301.287 34.1541 301.836C32.402 302.275 30.5806 302.362 28.7945 302.094L25.734 301.635C22.9263 301.214 20.195 300.386 17.6261 299.177L16.3807 298.591C14.1321 297.533 11.991 296.26 9.98701 294.79L9.49111 294.427C6.83838 292.481 4.55895 290.089 2.09661 287.908C1.81055 287.655 1.59632 287.5 1.5 287.5C1.20632 287.5 0.714998 285.703 0.342633 284.088C0.0997628 283.035 0 281.956 0 280.875V279.65C0 277.907 0.405793 276.188 1.18524 274.63C1.72488 273.55 2.43499 272.565 3.28824 271.712Z'
          fill={`url(#ufo-light-center-${color})`}
        />
        <linearGradient
          id={`ufo-light-center-${color}`}
          x1='114.5'
          y1='42'
          x2='21.9998'
          y2='280.5'
          gradientUnits='userSpaceOnUse'
        >
          <stop stopColor='#090D31' />
          <stop offset='0.197115' stopColor='#090D31' stopOpacity='0.3' />
          <stop
            className={cn(classes.ufoLightStop, classes[color])}
            offset='1'
            stopOpacity='0.52'
          />
        </linearGradient>
      </svg>

      <svg
        className={cn(classes.ufoLight, {
          [classes.ufoLightHidden]: hidden
        })}
        width='220'
        height='162'
        viewBox='0 0 220 162'
        fill='none'
        xmlns='http://www.w3.org/2000/svg'
      >
        <path
          d='M0.346063 120.577L0.421362 120.376C1.08455 118.608 2.67407 117.353 4.54824 117.119C5.17719 117.04 5.78588 116.845 6.34288 116.542L220 0.5V90L20.3904 156.371C19.1862 156.772 18.2063 157.661 17.6909 158.82L17.0364 160.293C16.7102 161.027 15.9824 161.5 15.1793 161.5C14.4501 161.5 13.7767 161.109 13.4149 160.476L12.2814 158.492C12.0942 158.165 11.8764 157.856 11.6309 157.569L9 154.5L8.44953 153.729C7.15184 151.913 5.99847 149.997 5 148C3.33882 144.345 2.10245 140.512 1.31516 136.576L1 135L0.315732 130.552C0.105551 129.186 0 127.806 0 126.424V122.486C0 121.834 0.11722 121.187 0.346063 120.577Z'
          fill={`url(#ufo-light-right-${color})`}
        />
        <defs>
          <linearGradient
            id={`ufo-light-right-${color}`}
            x1='12.5'
            y1='138.5'
            x2='182'
            y2='-10.5'
            gradientUnits='userSpaceOnUse'
          >
            <stop className={cn(classes.ufoLightStop, classes[color])} stopOpacity='0.5' />
            <stop offset='0.774038' stopColor='#090D31' stopOpacity='0.24' />
            <stop offset='1' stopColor='#090D31' />
          </linearGradient>
        </defs>
      </svg>
    </>
  )
}

export const Ufo = ({ color, direction, glowing }: UfoProps) => {
  const ufoColors: Record<'color', UfoColor>[] = [
    { color: 'default' },
    { color: 'green' },
    { color: 'red' }
  ]

  const ufoGreenRef = useRef<HTMLDivElement>(null)
  const ufoRedRef = useRef<HTMLDivElement>(null)

  const ufoRefs: Record<Exclude<UfoColor, 'default'>, RefObject<HTMLDivElement | null>> = {
    green: ufoGreenRef,
    red: ufoRedRef
  }

  useGlowingAnimation({
    elementsRef: [ufoGreenRef, ufoRedRef],
    enabled: glowing
  })

  return ufoColors.map(({ color: ufoColor }) => {
    return (
      <div
        key={ufoColor}
        ref={ufoColor !== 'default' ? ufoRefs[ufoColor] : null}
        className={cn(classes.ufo, {
          [classes.ufoFlipped]: direction === 'left',
          [classes.ufoVisible]: ufoColor === color
        })}
      >
        <Image
          key={ufoColor}
          className={cn(classes.ufoImg)}
          src={`/img/upgrades/ufo-right-${ufoColor}.png`}
          width={380}
          height={422}
          quality={100}
          alt='НЛО'
        />

        <UfoLights color={ufoColor} hidden={ufoColor === 'default'} />
      </div>
    )
  })
}
